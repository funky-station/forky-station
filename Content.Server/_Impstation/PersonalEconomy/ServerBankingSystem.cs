using System.Diagnostics.CodeAnalysis;
using Content.Server.Cargo.Systems;
using Content.Server.DeviceLinking.Systems;
using Content.Server.StationRecords.Systems;
using Content.Server.Stack;
using Content.Shared._Impstation.PersonalEconomy;
using Content.Shared._Impstation.PersonalEconomy.Components;
using Content.Shared._Impstation.PersonalEconomy.Events;
using Content.Shared._Impstation.PersonalEconomy.Systems;
using Content.Shared.Cargo.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.GameTicking;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.Stacks;
using Content.Shared.CCVar;
using Content.Shared.Station;
using Content.Shared.StationRecords;
using Robust.Server.GameStates;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Impstation.PersonalEconomy;

/// <summary>
/// This handles...
/// </summary>
public sealed class ServerBankingSystem : SharedBankingSystem
{
    [Dependency] private SharedTransformSystem _xform = null!;
    [Dependency] private PvsOverrideSystem _pvsOverride = null!;
    [Dependency] private IRobustRandom _random = null!;
    [Dependency] private SharedStationSystem _station = null!;
    [Dependency] private IPrototypeManager _proto = null!;
    [Dependency] private InventorySystem _inventory = null!;
    [Dependency] private StackSystem _stack = null!;
    [Dependency] private ItemSlotsSystem _itemSlots = null!;
    [Dependency] private SharedHandsSystem _hands = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private DeviceLinkSystem _deviceLink = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private CargoSystem _cargo = default!;
    [Dependency] private SharedUserInterfaceSystem _ui = default!;

    private readonly EntProtoId _bankAccountProto = "BankAccount";
    private readonly ProtoId<StackPrototype> _scripStack = "Scrip";

    // job, stacked salary
    private readonly Dictionary<string, int> _salaryByJob = new();

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BankCardComponent, ComponentInit>(OnComponentInit);
        // after StationRecordsSystem so the record key is already stamped onto the PDA
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawnComplete, after: [typeof(StationRecordsSystem)]);

        SubscribeLocalEvent<ATMComponent, WithdrawMessage>(OnWithdraw);
        SubscribeLocalEvent<ATMComponent, DepositMessage>(OnDeposit);
        SubscribeLocalEvent<AccountManagementConsoleComponent, ConvertStationScripMessage>(OnConvertStationScrip);
        SubscribeLocalEvent<AccountManagementConsoleComponent, CreateBusinessAccountMessage>(OnCreateBusinessAccount);
        SubscribeLocalEvent<PosSystemComponent, UnlockPosMessage>(OnUnlockPos);
        SubscribeLocalEvent<PoSWiredMessage>(OnSignal);

        SubscribeNetworkEvent<RequestBankPinEvent>(OnRequestPin);

        PopulateSalaries();
    }

    // command can cash the stations scrip reserves into spesos cargos bank acc
    private void OnConvertStationScrip(Entity<AccountManagementConsoleComponent> ent, ref ConvertStationScripMessage args)
    {
        if (!_cfg.GetCVar(CCVars.ScripStationCashout))
            return;

        if (!ConsoleAllowed(ent, args.Actor) || args.Amount <= 0)
            return;

        var station = _station.GetOwningStation(ent);
        if (station == null
            || !TryComp<StationPayrollComponent>(station, out var payroll)
            || !TryGetAccount(payroll.StationAccount, out var account)
            || !TryComp<StationBankAccountComponent>(station, out var cargoBank))
            return;

        // only convert whole spesos worth, and only debit the scrip actually used
        var spesos = args.Amount / AccountManagementConsoleComponent.CashoutRate;
        if (spesos <= 0)
            return;

        var scripUsed = spesos * AccountManagementConsoleComponent.CashoutRate;
        if (account.Value.Comp.Balance < scripUsed)
            return;

        // pull scrip from the payroll pool, drop the spesos into cargos account
        AdjustBalanceWithLog(payroll.StationAccount, -scripUsed, Loc.GetString("nanobank-cash"), Loc.GetString("nanobank-scrip-cashout-reason"));
        _cargo.UpdateBankAccount((station.Value, cargoBank), spesos, cargoBank.PrimaryAccount);
    }

    // creates an account for a business so players can register stuff separate from their personal acc
    private void OnCreateBusinessAccount(Entity<AccountManagementConsoleComponent> ent, ref CreateBusinessAccountMessage args)
    {
        if (!ConsoleAllowed(ent, args.Actor))
            return;

        var name = args.Name.Trim();
        if (name.Length == 0)
            return;

        // card needs to be slotted so it can mint it there
        var cardUid = _itemSlots.GetItemOrNull(ent, ent.Comp.CardSlotId);
        if (cardUid == null || !TryComp<BankCardComponent>(cardUid, out var cardComp))
            return;

        var account = CreateNewAccount(name, ResolveAccountParent(ent));
        WriteAccountToCard((cardUid.Value, cardComp), account.Comp.AccountNumber);
    }

    // first valid unlock claims the pos system for that account
    private void OnUnlockPos(Entity<PosSystemComponent> ent, ref UnlockPosMessage args)
    {
        AccountNumber ownerToCheck;
        if (ent.Comp.OwnerAccount.Number == 0)
        {
            if (!TryGetHeldCard(args.Actor, out var card))
                return;
            if (!TryGetAccount(card.Comp.AccountNumber, out var claimer))
                return;
            if (args.Pin != claimer.Value.Comp.Pin.Number)
                return;

            ent.Comp.OwnerAccount = claimer.Value.Comp.AccountNumber;
            Dirty(ent);
            ownerToCheck = claimer.Value.Comp.AccountNumber;
        }
        else
        {
            ownerToCheck = ent.Comp.OwnerAccount;
        }

        if (!TryGetAccount(ownerToCheck, out var owner) || args.Pin != owner.Value.Comp.Pin.Number)
            return;

        // tell just this client to open the merchant view
        _ui.ServerSendUiMessage(ent.Owner, POSUIKey.Key, new PosUnlockedMessage(), args.Actor);
    }

    private void OnSignal(PoSWiredMessage msg)
    {
        _deviceLink.InvokePort(msg.Ent.Owner, msg.Success ? msg.Ent.Comp.SuccessPort : msg.Ent.Comp.FailPort);

        // KACHING BABY!!!!!!!!!!!!
        _audio.PlayPvs(msg.Success ? msg.Ent.Comp.PurchaseSound : msg.Ent.Comp.DeclineSound, msg.Ent.Owner);
    }

    private void OnWithdraw(Entity<ATMComponent> ent, ref WithdrawMessage args)
    {
        if (args.Amount <= 0)
            return;

        if (!TryGetSlotAccount(ent, out var account))
            return;

        // withdrawing requires the account PIN
        if (args.Pin != account.Value.Comp.Pin.Number)
            return;

        if (account.Value.Comp.Balance < args.Amount)
            return;

        // take from account, log it, then hand over physical scrip
        AdjustBalanceWithLog(account.Value.Comp.AccountNumber, -args.Amount, Loc.GetString("nanobank-cash"), Loc.GetString("nanobank-withdrawal-reason"));
        var stack = _stack.SpawnNextToOrDrop(args.Amount, _scripStack, ent.Owner);
        _hands.PickupOrDrop(args.Actor, stack);
    }

    private void OnDeposit(Entity<ATMComponent> ent, ref DepositMessage args)
    {
        if (!TryGetSlotAccount(ent, out var account))
            return;

        var total = 0;
        foreach (var held in _hands.EnumerateHeld(args.Actor))
        {
            if (!TryComp<StackComponent>(held, out var stack) || stack.StackTypeId != _scripStack)
                continue;

            total += stack.Count;
            QueueDel(held);
        }

        if (total <= 0)
            return;

        AdjustBalanceWithLog(account.Value.Comp.AccountNumber, total, Loc.GetString("nanobank-cash"), Loc.GetString("nanobank-deposit-reason"));
    }

    private bool TryGetSlotAccount(Entity<ATMComponent> ent, [NotNullWhen(true)] out Entity<BankAccountComponent>? account)
    {
        account = null;

        var cardUid = _itemSlots.GetItemOrNull(ent, ent.Comp.CardSlotId);
        if (cardUid == null || !TryComp<BankCardComponent>(cardUid, out var card))
            return false;

        return TryGetAccount(card.AccountNumber, out account);
    }

    // link the account to its owner's station record so payroll can read criminal status.
    // the record key lives on the PDA in the id slot, which is also where the bank card is
    private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
    {
        if (!TryGetOwnedAccount(ev.Mob, out var account))
            return;

        // bind the account to this player so only they can be told the PIN later
        account.Value.Comp.OwnerUserId = ev.Player.UserId;

        // tell the owner their PIN once; it's never printed on the card, so this is how they learn it
        _popup.PopupEntity(
            Loc.GetString("bank-pin-notify",
                ("account", $"{account.Value.Comp.AccountNumber.Number:000000}"),
                ("pin", $"{account.Value.Comp.Pin.Number:0000}")),
            ev.Mob, ev.Mob, PopupType.Medium);

        // is tjere a better way to do this
        RaiseNetworkEvent(
            new BankPinResponseEvent(account.Value.Comp.AccountNumber.Number, account.Value.Comp.Pin.Number),
            ev.Player);

        // link the station record (for criminal-status pay withholding) if one exists
        if (_inventory.TryGetSlotEntity(ev.Mob, "id", out var idUid)
            && TryComp<StationRecordKeyStorageComponent>(idUid, out var keyStorage)
            && keyStorage.Key is { } key)
        {
            account.Value.Comp.StationRecordId = key.Id;
        }
    }

    // the pin needs to be kept secret so thats why we request it this way
    private void OnRequestPin(RequestBankPinEvent ev, EntitySessionEventArgs args)
    {
        if (args.SenderSession.AttachedEntity is not { } player)
            return;

        if (!TryGetOwnedAccount(player, out var account))
            return;

        if (account.Value.Comp.OwnerUserId != args.SenderSession.UserId)
            return;

        RaiseNetworkEvent(
            new BankPinResponseEvent(account.Value.Comp.AccountNumber.Number, account.Value.Comp.Pin.Number),
            args.SenderSession);
    }

    // resolves the account belonging to a player via the bank card in their PDA
    private bool TryGetOwnedAccount(EntityUid player, [NotNullWhen(true)] out Entity<BankAccountComponent>? account)
    {
        account = null;

        if (!_inventory.TryGetSlotEntity(player, "id", out var idUid))
            return false;
        if (!TryComp<PdaComponent>(idUid, out var pda)
            || pda.BankCardSlot.ContainerSlot?.ContainedEntity is not { } card
            || !TryComp<BankCardComponent>(card, out var bankCard))
            return false;

        return TryGetAccount(bankCard.AccountNumber, out account);
    }

    // sums every PaymentSalaryPrototype a job appears in (e.g. base wage + hazard pay)
    private void PopulateSalaries()
    {
        _salaryByJob.Clear();
        foreach (var proto in _proto.EnumeratePrototypes<PaymentSalaryPrototype>())
        {
            foreach (var role in proto.Roles)
            {
                _salaryByJob[role] = _salaryByJob.GetValueOrDefault(role.Id) + proto.Salary;
            }
        }
    }

    /// <summary>
    /// Stacked salary for a job, or 0 if no salary prototype covers it.
    /// </summary>
    public int GetSalaryForJob(string jobId)
    {
        return _salaryByJob.GetValueOrDefault(jobId);
    }

    //todo should this be a different event?
    private void OnComponentInit(Entity<BankCardComponent> ent, ref ComponentInit args)
    {
        // blank cards stay account-less until written at a console
        if (!ent.Comp.AutoCreateAccount)
            return;

        SetupID(ent);
    }

    private void SetupID(Entity<BankCardComponent> ent)
    {
        var account = CreateNewAccount("Unknown", ResolveAccountParent(ent));
        ent.Comp.AccountNumber = account.Comp.AccountNumber;
        SetAccountSalary(account.Comp.AccountNumber, ent.Comp.Salary);
        SetAccountBalance(account.Comp.AccountNumber, ent.Comp.StartingBalance);
        Dirty(ent);
    }

    // accounts get parented to the station so they're cleaned up with it
    // i didnt like the _cheeseWorld sorry
    private EntityUid? ResolveAccountParent(EntityUid source)
    {
        var owning = _station.GetOwningStation(source);
        if (owning != null)
            return owning;

        //fallback: any station. if there isn't one, leave it on the source's current parent
        var stations = _station.GetStations();
        return stations.Count > 0 ? stations[0] : null;
    }

    public Entity<BankAccountComponent> CreateNewAccount(string name, EntityUid? parent)
    {
        //generate a unique account number (the public address)
        var accountNo = _random.Next(1, 1000000);
        while (TryGetAccount(accountNo, out _))
        {
            accountNo = _random.Next(1, 1000000);
        }

        //the PIN is a 4-digit secret; it doesn't need to be unique since it's never used for lookup
        var pin = _random.Next(1000, 10000);

        var newAccount = Spawn(_bankAccountProto);
        if (parent != null)
            _xform.SetParent(newAccount, parent.Value);

        // TAYDEO NOTE:
        // maybe they dont? original funky bank code just relied on a manual look up at an ATM, since clients didnt
        // really need to know anything? maybe go back to this architecture? We'll See.
        // LATE NIGHT EDIT: WE ENDED UP KINDA DOING THIS HAHA
        _pvsOverride.AddForceSend(newAccount);

        //create new account
        var bankComp = Comp<BankAccountComponent>(newAccount);

        var oldNumber = bankComp.AccountNumber;
        bankComp.AccountNumber = accountNo;
        bankComp.Pin = pin;
        bankComp.Name = name;
        ReindexAccount((newAccount, bankComp), oldNumber);

        //and send the comp back off to the client
        Dirty<BankAccountComponent>((newAccount, bankComp));
        return (newAccount, bankComp);
    }
}
