using Content.Client._Funkystation.Unions.UI;
using Content.Shared._Funkystation.Unions;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.Unions;

public sealed class UnionClipboardClaimBoundUserInterface(EntityUid owner, Enum uiKey)
    : BoundUserInterface(owner, uiKey)
{
    [ViewVariables] private UnionClipboardClaimMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<UnionClipboardClaimMenu>();
        _menu.OnConfirm += () => SendMessage(new UnionClipboardClaimLeadershipConfirmMessage());
    }
}
