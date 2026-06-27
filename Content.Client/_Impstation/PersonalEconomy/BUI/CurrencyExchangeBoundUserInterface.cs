using Content.Client._Impstation.PersonalEconomy.UI;
using Content.Shared._Impstation.PersonalEconomy.Components;
using Content.Shared._Impstation.PersonalEconomy.Events;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Stacks;
using Robust.Client.UserInterface;

namespace Content.Client._Impstation.PersonalEconomy.BUI;

public sealed class CurrencyExchangeBoundUserInterface : BoundUserInterface
{
    private readonly ItemSlotsSystem _itemSlots;

    private CurrencyExchangeMenu? _menu;

    public CurrencyExchangeBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _itemSlots = EntMan.System<ItemSlotsSystem>();
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<CurrencyExchangeMenu>();
        _menu.GetState = GetState;
        _menu.OnInsertPressed += () => SendPredictedMessage(new InsertCashMessage());
        _menu.OnEjectPressed += () => SendPredictedMessage(new EjectCashMessage());
        _menu.OnConvertPressed += () => SendPredictedMessage(new ConvertCurrencyMessage());
    }

    private (int Count, bool IsScrip, int Tax, int ScripPerSpeso)? GetState()
    {
        if (!EntMan.TryGetComponent<CurrencyExchangeComponent>(Owner, out var exchange))
            return null;

        var cashUid = _itemSlots.GetItemOrNull(Owner, exchange.CashSlotId);
        if (cashUid == null || !EntMan.TryGetComponent<StackComponent>(cashUid, out var stack))
            return null;

        return (stack.Count, stack.StackTypeId == "Scrip", exchange.TaxPercent, exchange.ScripPerSpeso);
    }
}
