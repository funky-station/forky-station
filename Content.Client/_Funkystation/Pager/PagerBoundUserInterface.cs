using Content.Shared._Funkystation.Pager;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.Pager;

public sealed class PagerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    [ViewVariables]
    private PagerWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<PagerWindow>();
        _window.OnSend += (targetNumber, code) =>
        {
            SendMessage(new PagerSendPageMessage(targetNumber, code));
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is PagerBoundUserInterfaceState pagerState)
            _window?.UpdateState(pagerState);
    }
}
