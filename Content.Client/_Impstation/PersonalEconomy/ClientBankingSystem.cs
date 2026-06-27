using Content.Client.CharacterInfo;
using Content.Shared._Impstation.PersonalEconomy.Components;
using Content.Shared._Impstation.PersonalEconomy.Events;
using Content.Shared._Impstation.PersonalEconomy.Systems;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Impstation.PersonalEconomy;

public sealed class ClientBankingSystem : SharedBankingSystem
{
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

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
        if (!_inventory.TryGetSlotEntity(player, "id", out var idUid))
            return null;
        if (!TryComp<PdaComponent>(idUid, out var pda)
            || pda.BankCardSlot.ContainerSlot?.ContainedEntity is not { } card
            || !TryComp<BankCardComponent>(card, out var bankCard))
            return null;

        return TryGetAccount(bankCard.AccountNumber, out var account) ? account : null;
    }
}
