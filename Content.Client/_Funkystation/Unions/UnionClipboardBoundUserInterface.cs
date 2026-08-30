using Content.Client._Funkystation.Unions.UI;
using Content.Shared._Funkystation.Unions;
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
        _menu.OnRemoveMember += target => SendMessage(new UnionClipboardRemoveMemberMessage(target));
        _menu.OnAddNote += (target, title, text) => SendMessage(new UnionClipboardAddNoteMessage(target, title, text));
        _menu.OnBeginSteward += target => SendMessage(new UnionClipboardBeginStewardMessage(target));
        _menu.OnCancelSteward += () => SendMessage(new UnionClipboardCancelStewardMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not UnionClipboardBoundUserInterfaceState clipboardState)
            return;

        _menu?.UpdateState(clipboardState);
    }
}
