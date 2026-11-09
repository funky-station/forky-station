using Content.Client.CharacterInfo;
using Content.Shared._Impstation.PersonalEconomy;
using Content.Shared._Impstation.PersonalEconomy.Components;
using Content.Shared._Impstation.PersonalEconomy.Events;
using Content.Shared._Impstation.PersonalEconomy.Systems;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;

namespace Content.Client._Impstation.PersonalEconomy;

public sealed class ClientBankingSystem : SharedBankingSystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private SharedUserInterfaceSystem _ui = default!;

    private int? _playerPin;
    private Label? _pinLabel;

    public int? LocalAccount { get; private set; }

    // is the local player holding the card for this account in their hand right now?
    public bool LocalHoldsAccount(int account)
    {
        return _player.LocalEntity is { } ent
            && TryGetHeldCard(ent, out var card)
            && card.Comp.AccountNumber.Number == account;
    }

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CharacterInfoSystem.GetCharacterInfoControlsEvent>(OnGetCharacterInfoControls);
        SubscribeNetworkEvent<BankPinResponseEvent>(OnPinResponse);

        SubscribeLocalEvent<ATMComponent, EntInsertedIntoContainerMessage>(OnAtmCardInserted);
        SubscribeLocalEvent<ATMComponent, EntRemovedFromContainerMessage>(OnAtmCardRemoved);
        SubscribeLocalEvent<BankAccountComponent, AfterAutoHandleStateEvent>(OnAccountState);
        SubscribeLocalEvent<StationPayrollComponent, AfterAutoHandleStateEvent>(OnPayrollState);
    }

    private void OnAtmCardInserted(Entity<ATMComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        UpdateAtmUi(ent);
    }

    private void OnAtmCardRemoved(Entity<ATMComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        UpdateAtmUi(ent);
    }

    private void OnAccountState(Entity<BankAccountComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        UpdateAllAtmUis();
    }

    private void OnPayrollState(Entity<StationPayrollComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        UpdateAllAtmUis();
    }

    private void UpdateAllAtmUis()
    {
        var query = EntityQueryEnumerator<ATMComponent>();
        while (query.MoveNext(out var uid, out _))
        {
            UpdateAtmUi(uid);
        }
    }

    private void UpdateAtmUi(EntityUid uid)
    {
        if (_ui.TryGetOpenUi(uid, ATMUIKey.Key, out var bui))
            bui.Update();
    }

    private void OnPinResponse(BankPinResponseEvent ev)
    {
        LocalAccount = ev.AccountNumber;
        _playerPin = ev.Pin;
        _pinLabel?.Text = Loc.GetString("bank-character-pin", ("pin", $"{ev.Pin:0000}"));
    }

    // adds the player's account number and PIN to character info so they always have it
    private void OnGetCharacterInfoControls(ref CharacterInfoSystem.GetCharacterInfoControlsEvent ev)
    {
        var accountNumber = LocalAccount;
        if (accountNumber is null && GetOwnedAccount(ev.Entity) is { } account)
            accountNumber = account.Comp.AccountNumber.Number;

        if (accountNumber is null)
            return;

        var box = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
            Margin = new Thickness(0, 6, 0, 0),
        };
        box.AddChild(new Label
        {
            Text = Loc.GetString("bank-character-heading"),
            StyleClasses = { "LabelHeading" },
        });
        box.AddChild(new Label
        {
            Text = Loc.GetString("bank-character-account", ("number", $"{accountNumber.Value:000000}")),
        });

        _pinLabel = new Label
        {
            Text = Loc.GetString("bank-character-pin", ("pin", _playerPin is { } p ? $"{p:0000}" : "----")),
        };
        box.AddChild(_pinLabel);

        ev.Controls.Add(box);
    }

    private Entity<BankAccountComponent>? GetOwnedAccount(EntityUid player)
    {
        if (!TryGetIdBankCard(player, out var card))
            return null;

        return TryGetAccount(card.Comp.AccountNumber, out var account) ? account : null;
    }
}
