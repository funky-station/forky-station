using Content.Client._Funkystation.Unions.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.Unions;

public sealed class UnionClipboardBoundUserInterface(EntityUid owner, Enum uiKey, UnionClipboardMenu? menu)
    : BoundUserInterface(owner, uiKey)
{
    [ViewVariables] private UnionClipboardMenu? _menu = menu;
    
    protected override void Open()
    {
        base.Open();
        
        _menu = this.CreateWindow<UnionClipboardMenu>();
    }
}