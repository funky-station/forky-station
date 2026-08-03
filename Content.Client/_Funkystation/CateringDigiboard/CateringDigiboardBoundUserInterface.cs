using Content.Client._Funkystation.CateringDigiboard.UI;
using Content.Shared._Funkystation.CateringDigiboard;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.CateringDigiboard;

[UsedImplicitly]
public sealed class CateringDigiboardBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private CateringDigiboardMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<CateringDigiboardMenu>();
        _menu.OnAddItem += () => SendMessage(new CateringDigiboardAddItemMessage());
        _menu.OnAddCategory += () => SendMessage(new CateringDigiboardAddCategoryMessage());
        _menu.OnEditItem += (id, name, description, cost) =>
            SendMessage(new CateringDigiboardEditItemMessage(id, name, description, cost));
        _menu.OnRemoveItem += id => SendMessage(new CateringDigiboardRemoveItemMessage(id));
        _menu.OnReorderItem += (id, index) => SendMessage(new CateringDigiboardReorderMessage(id, index));
        _menu.OnPrint += count => SendMessage(new CateringDigiboardPrintMessage(count));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not CateringDigiboardBoundUserInterfaceState msg)
            return;

        _menu?.UpdateState(msg);
    }
}
