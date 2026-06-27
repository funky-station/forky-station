using System.Diagnostics.CodeAnalysis;
using Content.Shared.CCVar;
using Content.Shared._Impstation.PersonalEconomy.Components;
using Content.Shared._Impstation.PersonalEconomy.Events;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Station;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;

namespace Content.Shared._Impstation.PersonalEconomy.Systems;

/// <summary>
/// The main banking system; handles all funds transfers and keeps track of bank accounts etc etc
/// </summary>
public abstract class SharedBankingSystem : EntitySystem
{
    [Dependency] private readonly MetaDataSystem _metaData = null!;
    [Dependency] private readonly IGameTiming _timing = null!;
    [Dependency] private readonly ItemSlotsSystem _itemSlots = null!;
    [Dependency] private readonly SharedHandsSystem _hands = null!;
    [Dependency] private readonly AccessReaderSystem _accessReader = null!;
    [Dependency] private readonly SharedPopupSystem _popup = null!;
    [Dependency] private readonly SharedStationSystem _station = null!;
    [Dependency] private readonly IConfigurationManager _cfg = null!;

    // we dont want command being targetted in this stuff
    private const string CommandDepartment = "Command";

    //lookup so we're not full-scanning every transaction
    private readonly Dictionary<int, EntityUid> _accountsByNumber = new();

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<BankCardComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<ATMComponent, RequestTransactionMessage>(OnTransactionRequested);
        SubscribeLocalEvent<ATMComponent, InsertCardMessage>(OnInsertCardRequested);
        SubscribeLocalEvent<ATMComponent, EjectCardMessage>(OnEjectCardRequested);
        SubscribeLocalEvent<PosSystemComponent, UpdatePoSSettingsMessage>(OnPoSSettingsUpdate);
        SubscribeLocalEvent<PosSystemComponent, PoSTransactionSuccededMessage>(OnTransactionSucceded);
        SubscribeLocalEvent<PosSystemComponent, PoSTransactionFailedMessage>(OnTransactionFailed);
        SubscribeLocalEvent<PosSystemComponent, PoSTipMessage>(OnTip);

        SubscribeLocalEvent<AccountManagementConsoleComponent, SetAccountStatusMessage>(OnSetAccountStatus);
        SubscribeLocalEvent<AccountManagementConsoleComponent, SetAccountSalaryMessage>(OnSetAccountSalary);
        SubscribeLocalEvent<AccountManagementConsoleComponent, GrantAccountBonusMessage>(OnGrantBonus);
        SubscribeLocalEvent<AccountManagementConsoleComponent, InsertCardMessage>(OnConsoleInsertCard);
        SubscribeLocalEvent<AccountManagementConsoleComponent, EjectCardMessage>(OnConsoleEjectCard);
        SubscribeLocalEvent<AccountManagementConsoleComponent, WriteCardMessage>(OnWriteCard);
        SubscribeLocalEvent<AccountManagementConsoleComponent, SetDepartmentStatusMessage>(OnSetDepartmentStatus);
        SubscribeLocalEvent<AccountManagementConsoleComponent, GrantDepartmentBonusMessage>(OnGrantDepartmentBonus);

        SubscribeLocalEvent<BankAccountComponent, ComponentStartup>(OnAccountStartup);
        SubscribeLocalEvent<BankAccountComponent, ComponentShutdown>(OnAccountShutdown);
    }

    private void OnAccountStartup(Entity<BankAccountComponent> ent, ref ComponentStartup args)
    {
        _accountsByNumber[ent.Comp.AccountNumber] = ent;
    }

    private void OnAccountShutdown(Entity<BankAccountComponent> ent, ref ComponentShutdown args)
    {
        _accountsByNumber.Remove(ent.Comp.AccountNumber);
    }

    /// <summary>
    /// makes sure cache is insync
    /// </summary>
    protected void ReindexAccount(Entity<BankAccountComponent> ent, AccountNumber oldNumber)
    {
        if (oldNumber == ent.Comp.AccountNumber)
            return;

        _accountsByNumber.Remove(oldNumber);
        _accountsByNumber[ent.Comp.AccountNumber] = ent;
    }

    private void OnTransactionFailed(Entity<PosSystemComponent> ent, ref PoSTransactionFailedMessage args)
    {
        SignalPosTransaction(ent, false);
    }

    private void OnTransactionSucceded(Entity<PosSystemComponent> ent, ref PoSTransactionSuccededMessage args)
    {
        //customer pays with the card in their hand. they must cover the subtotal plus tax
        var tax = PosTaxFor(ent.Comp.Amount);
        var total = ent.Comp.Amount + tax;
        var success = TryGetHeldCard(args.Actor, out var card)
            && TryGetAccount(card.Comp.AccountNumber, out var customer)
            && customer.Value.Comp.Balance >= total
            && TryMakeTransaction(card.Comp.AccountNumber, ent.Comp.RecipientAccount, total, ent.Comp.Reason, ent.Comp.MerchantName);

        // the tax is skimmed to the stations scrip pool, on top of the merchants cut
        if (success && tax > 0 && TryGetStationScripAccount(ent, out var stationAccount))
            TryMakeTransaction(ent.Comp.RecipientAccount, stationAccount, tax, Loc.GetString("pos-tax-reason"));

        SignalPosTransaction(ent, success);
    }

    private void OnTip(Entity<PosSystemComponent> ent, ref PoSTipMessage args)
    {
        if (args.Amount <= 0 || !TryGetHeldCard(args.Actor, out var card))
            return;

        //the tip is an extra transfer straight to the merchant
        TryMakeTransaction(card.Comp.AccountNumber, ent.Comp.RecipientAccount, args.Amount, Loc.GetString("pos-tip-reason"), ent.Comp.MerchantName);
    }

    // tax owed; the cvar is a percentage (4.95 = 4.95%)
    public int PosTaxFor(int subtotal)
    {
        return (int) MathF.Round(subtotal * _cfg.GetCVar(CCVars.PosTax) / 100f);
    }

    private bool TryGetStationScripAccount(EntityUid pos, out AccountNumber account)
    {
        account = default;
        var station = _station.GetOwningStation(pos);
        if (station == null || !TryComp<StationPayrollComponent>(station, out var payroll) || payroll.StationAccount.Number == 0)
            return false;

        account = payroll.StationAccount;
        return true;
    }

    private void SignalPosTransaction(Entity<PosSystemComponent> ent, bool success)
    {
        RaiseLocalEvent(new PoSWiredMessage(ent, success));
    }

    private void OnPoSSettingsUpdate(Entity<PosSystemComponent> ent, ref UpdatePoSSettingsMessage args)
    {
        ent.Comp.RecipientAccount = args.Recipient;
        ent.Comp.Amount = args.Amount;
        ent.Comp.Reason = args.Reason;
        ent.Comp.MerchantName = args.MerchantName;

        Dirty(ent);
    }

    private void OnTransactionRequested(Entity<ATMComponent> ent, ref RequestTransactionMessage args)
    {
        var cardUid = _itemSlots.GetItemOrNull(ent, ent.Comp.CardSlotId);
        if (cardUid == null || !TryComp<BankCardComponent>(cardUid, out var card))
            return;

        TryMakeTransaction(card.AccountNumber, args.RecipientAccount, args.Amount, args.Reason);
    }

    private void OnInsertCardRequested(Entity<ATMComponent> ent, ref InsertCardMessage args)
    {
        if (!TryGetHeldCard(args.Actor, out var card))
            return;

        _itemSlots.TryInsert(ent, ent.Comp.CardSlotId, card.Owner, args.Actor);
    }

    private void OnEjectCardRequested(Entity<ATMComponent> ent, ref EjectCardMessage args)
    {
        // pickup into the player's hand or just falls on floor lol
        if (!_itemSlots.TryGetSlot(ent, ent.Comp.CardSlotId, out var slot))
            return;

        _itemSlots.TryEjectToHands(ent, slot, args.Actor);
    }

    // the console only honors management actions for crew with command access, like the comms console
    protected bool ConsoleAllowed(EntityUid console, EntityUid actor)
    {
        if (!TryComp<AccessReaderComponent>(console, out var reader) || _accessReader.IsAllowed(actor, console, reader))
            return true;

        _popup.PopupClient(Loc.GetString("account-management-access-denied"), console, actor);
        return false;
    }

    private void OnSetAccountStatus(Entity<AccountManagementConsoleComponent> ent, ref SetAccountStatusMessage args)
    {
        if (!ConsoleAllowed(ent, args.Actor))
            return;

        SetAccountStatus(args.Account, args.Status, args.Reason);
    }

    private void OnSetAccountSalary(Entity<AccountManagementConsoleComponent> ent, ref SetAccountSalaryMessage args)
    {
        if (!ConsoleAllowed(ent, args.Actor) || args.Salary < 0)
            return;

        SetAccountSalary(args.Account, args.Salary);
    }

    private void OnGrantBonus(Entity<AccountManagementConsoleComponent> ent, ref GrantAccountBonusMessage args)
    {
        if (!ConsoleAllowed(ent, args.Actor))
            return;

        GrantBonus(args.Account, args.Amount);
    }

    private void OnConsoleInsertCard(Entity<AccountManagementConsoleComponent> ent, ref InsertCardMessage args)
    {
        if (!ConsoleAllowed(ent, args.Actor) || !TryGetHeldCard(args.Actor, out var card))
            return;

        _itemSlots.TryInsert(ent, ent.Comp.CardSlotId, card.Owner, args.Actor);
    }

    private void OnConsoleEjectCard(Entity<AccountManagementConsoleComponent> ent, ref EjectCardMessage args)
    {
        if (!ConsoleAllowed(ent, args.Actor) || !_itemSlots.TryGetSlot(ent, ent.Comp.CardSlotId, out var slot))
            return;

        _itemSlots.TryEjectToHands(ent, slot, args.Actor);
    }

    private void OnWriteCard(Entity<AccountManagementConsoleComponent> ent, ref WriteCardMessage args)
    {
        if (!ConsoleAllowed(ent, args.Actor))
            return;

        var cardUid = _itemSlots.GetItemOrNull(ent, ent.Comp.CardSlotId);
        if (cardUid == null || !TryComp<BankCardComponent>(cardUid, out var cardComp))
            return;

        WriteAccountToCard((cardUid.Value, cardComp), args.Account);
    }

    private void OnSetDepartmentStatus(Entity<AccountManagementConsoleComponent> ent, ref SetDepartmentStatusMessage args)
    {
        if (!ConsoleAllowed(ent, args.Actor))
            return;

        //command roles are exempt from department-wide suspension unless command itself is the target
        var protectCommand = args.Status == PaymentStatus.Suspended && args.Department != CommandDepartment;

        var query = EntityQueryEnumerator<BankAccountComponent>();
        while (query.MoveNext(out _, out var comp))
        {
            if (comp.Department != args.Department)
                continue;
            if (protectCommand && comp.IsCommand)
                continue;

            SetAccountStatus(comp.AccountNumber, args.Status, args.Reason);
        }
    }

    private void OnGrantDepartmentBonus(Entity<AccountManagementConsoleComponent> ent, ref GrantDepartmentBonusMessage args)
    {
        if (!ConsoleAllowed(ent, args.Actor) || args.Amount <= 0)
            return;

        var query = EntityQueryEnumerator<BankAccountComponent>();
        while (query.MoveNext(out _, out var comp))
        {
            if (comp.Department == args.Department)
                GrantBonus(comp.AccountNumber, args.Amount);
        }
    }

    // they gotta actually hold the card in their hand
    protected bool TryGetHeldCard(EntityUid actor, out Entity<BankCardComponent> card)
    {
        card = default;
        foreach (var held in _hands.EnumerateHeld(actor))
        {
            if (TryComp<BankCardComponent>(held, out var comp))
            {
                card = (held, comp);
                return true;
            }
        }
        return false;
    }

    public bool TryMakeTransaction(AccountNumber sender, AccountNumber recipient, int amount, string reason, string? recipientNameOverride = null)
    {
        //todo need to do something for if a transaction becomes invalid after a client confirms it
        if (!VerifyTransaction(sender, recipient, amount))
            return false;

        MakeTransaction(sender, recipient, amount, reason, recipientNameOverride);
        return true;

    }

    private bool VerifyTransaction(AccountNumber sender, AccountNumber recipient, int amount)
    {
        //no Negative Cheese
        if (amount <= 0)
            return false;

        //return false if neither account exists
        if (!TryGetAccount(sender, out var senderAccount) ||
            !TryGetAccount(recipient, out var recipientAccount))
            return false;

        //return true if the sender has enough money
        return senderAccount.Value.Comp.Balance >= amount;
    }

    private void MakeTransaction(AccountNumber sender, AccountNumber recipient, int amount, string reason, string? recipientNameOverride = null)
    {
        //this should always be true by the time this gets called but
        //could make these out variables from the verify method, maybe?
        //will matter less when I've got a proper cache in buuuuuut
        if (!TryGetAccount(sender, out var senderAccount) || !TryGetAccount(recipient, out var recipientAccount))
            return;

        //adjust balances
        senderAccount.Value.Comp.Balance -= amount;
        recipientAccount.Value.Comp.Balance += amount;

        //add transactions! the "other account" recorded is each party's public account number.
        //the sender sees the override name (pos merchant name) if one was given
        var senderSeesName = string.IsNullOrWhiteSpace(recipientNameOverride) ? recipientAccount.Value.Comp.Name : recipientNameOverride;
        AddTransaction(senderAccount.Value, senderSeesName, -amount, recipient, reason);
        AddTransaction(recipientAccount.Value, senderAccount.Value.Comp.Name, amount, senderAccount.Value.Comp.AccountNumber, reason);
    }

    private void AddTransaction(Entity<BankAccountComponent> account, string otherName, int amount, int from, string reason)
    {
        //limit reason length
        if (reason.Length > 64) //todo make this a cvar
            reason = reason[..64];

        var timestamp = _timing.CurTime.TotalSeconds;
        var transaction = new BankTransaction(from, otherName, amount, timestamp, reason);
        account.Comp.Transactions.Insert(0, transaction);
        //keep at most 10
        while (account.Comp.Transactions.Count > 10) //todo make this a cvar
        {
            account.Comp.Transactions.RemoveAt(account.Comp.Transactions.Count - 1);
        }

        Dirty(account);
    }

    private void OnExamined(Entity<BankCardComponent> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        //account number is the public address others send money to. the PIN is secret and never shown
        args.PushMarkup(Loc.GetString("bank-card-examine-account-number", ("number", $"{ent.Comp.AccountNumber.Number:000000}")),4);

        if (!TryGetAccount(ent.Comp.AccountNumber, out var account))
            return;

        args.PushMarkup("The two below are for testing!", 3);
        args.PushMarkup(Loc.GetString("bank-card-examine-balance", ("balance", account.Value.Comp.Balance)), 2); //todo remove this
        args.PushMarkup(Loc.GetString("bank-card-examine-salary", ("salary", account.Value.Comp.Salary)), 1); //todo remove this
    }

    public bool TryGetAccount(AccountNumber accountNumber, [NotNullWhen(true)] out Entity<BankAccountComponent>? account)
    {
        account = null;
        if (!_accountsByNumber.TryGetValue(accountNumber, out var uid))
            return false;
        if (!TryComp<BankAccountComponent>(uid, out var comp))
            return false;
        account = (uid, comp);
        return true;
    }

    /// <summary>
    /// Set the name for an account
    /// </summary>
    /// <param name="accessNumber"></param>
    /// <param name="name"></param>
    public virtual void SetAccountName(AccountNumber accessNumber, string name)
    {
        if (!TryGetAccount(accessNumber, out var account))
            return;

        account.Value.Comp.Name = name;
        Dirty(account.Value);
    }

    public virtual void SetAccountSalary(AccountNumber accessNumber, int salary)
    {
        if (!TryGetAccount(accessNumber, out var account))
            return;

        account.Value.Comp.Salary = salary;
        Dirty(account.Value);
    }

    public virtual void SetAccountBalance(AccountNumber accessNumber, int balance)
    {
        if (!TryGetAccount(accessNumber, out var account))
            return;

        account.Value.Comp.Balance = balance;
        Dirty(account.Value);
    }

    public virtual void SetAccountStatus(AccountNumber accessNumber, PaymentStatus status, string reason)
    {
        if (!TryGetAccount(accessNumber, out var account))
            return;

        account.Value.Comp.Status = status;
        account.Value.Comp.StatusReason = reason;
        Dirty(account.Value);
    }

    public virtual void SetAccountDepartment(AccountNumber accessNumber, string department, bool isCommand)
    {
        if (!TryGetAccount(accessNumber, out var account))
            return;

        account.Value.Comp.Department = department;
        account.Value.Comp.IsCommand = isCommand;
        Dirty(account.Value);
    }

    // adds funds to an account and logs it. used by the console for one-off bonuses
    public virtual void GrantBonus(AccountNumber accessNumber, int amount)
    {
        if (amount <= 0 || !TryGetAccount(accessNumber, out var account))
            return;

        account.Value.Comp.Balance += amount;
        AddTransaction(account.Value, Loc.GetString("nanobank-station-bank"), amount, 0, Loc.GetString("nanobank-bonus-reason"));
    }

    // shows a withheld deposit
    public void LogWithheld(AccountNumber accessNumber, string reason)
    {
        if (!TryGetAccount(accessNumber, out var account))
            return;

        AddTransaction(account.Value, Loc.GetString("nanobank-station-bank"), 0, 0, reason);
    }

    public void AdjustBalanceWithLog(AccountNumber accessNumber, int amount, string otherName, string reason)
    {
        if (!TryGetAccount(accessNumber, out var account))
            return;

        account.Value.Comp.Balance += amount;
        AddTransaction(account.Value, otherName, amount, 0, reason);
    }

    /// <summary>
    /// Update the details on a bank card to reflect the details of a given account.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="accessNumber"></param>
    public virtual void UpdateCardDetails(Entity<BankCardComponent> card, AccountNumber accessNumber)
    {
        if (!TryGetAccount(accessNumber, out var account))
            return;

        SetCardName(card, account.Value.Comp.Name);
        SetCardNumber(card, account.Value.Comp.AccountNumber);
    }

    // points a card at an existing account
    public virtual void WriteAccountToCard(Entity<BankCardComponent> card, AccountNumber accountNumber)
    {
        if (!TryGetAccount(accountNumber, out var account))
            return;

        var name = account.Value.Comp.Name;
        SetCardName(card, name); // also renames the entity from "blank bank card" to "{name}'s bank card"
        SetCardNumber(card, account.Value.Comp.AccountNumber);
        _metaData.SetEntityDescription(card, Loc.GetString("bank-card-description", ("name", name)));
    }

    /// <summary>
    /// Set the name on a card
    /// </summary>
    /// <param name="card"></param>
    /// <param name="name"></param>
    public virtual void SetCardName(Entity<BankCardComponent> card, string name)
    {
        card.Comp.Name = name;
        _metaData.SetEntityName(card, Loc.GetString(card.Comp.NamedLocId, ("name", name)));
        Dirty(card);
    }

    /// <summary>
    /// set the number on a card
    /// </summary>
    /// <param name="card"></param>
    /// <param name="accessNumber"></param>
    public virtual void SetCardNumber(Entity<BankCardComponent> card, AccountNumber accessNumber)
    {
        card.Comp.AccountNumber = accessNumber;
        Dirty(card);
    }
}
