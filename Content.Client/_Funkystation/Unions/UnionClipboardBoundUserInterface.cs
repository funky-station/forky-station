using Content.Client._Funkystation.Unions.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.Unions;

public sealed class UnionClipboardBoundUserInterface(EntityUid owner, Enum uiKey)
    : BoundUserInterface(owner, uiKey)
{
    [ViewVariables] private UnionClipboardMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<UnionClipboardMenu>();
    }
}
