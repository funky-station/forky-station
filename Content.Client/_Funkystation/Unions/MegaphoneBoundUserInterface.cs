using Content.Client._Funkystation.Unions.UI;
using Content.Shared._Funkystation.Unions;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.Unions;

public sealed class MegaphoneBoundUserInterface(EntityUid owner, Enum uiKey)
    : BoundUserInterface(owner, uiKey)
{
    [ViewVariables] private MegaphoneMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<MegaphoneMenu>();
        _menu.OnCallStrike += text => SendMessage(new MegaphoneCallStrikeMessage(text));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not MegaphoneBoundUserInterfaceState megaphoneState)
            return;

        _menu?.UpdateState(megaphoneState);
    }
}
