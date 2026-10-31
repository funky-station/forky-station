using Content.Client.UserInterface.Fragments;
using Content.Shared._Funkystation.Pager;
using Content.Shared.CartridgeLoader;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.Pager;

public sealed partial class PagerCartridgeUi : UIFragment
{
    private PagerCartridgeUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        _fragment = new PagerCartridgeUiFragment();

        _fragment.OnSend += (number, code) => Send(new PagerCartridgeSendMessage(number, code), userInterface);
        _fragment.OnMassPage += (prefix, code) => Send(new PagerCartridgeMassPageMessage(prefix, code), userInterface);
        _fragment.OnCycleMode += () => Send(new PagerCartridgeCycleModeMessage(), userInterface);
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is PagerCartridgeUiState cast)
            _fragment?.UpdateState(cast);
    }

    private static void Send(CartridgeMessageEvent message, BoundUserInterface userInterface)
    {
        userInterface.SendMessage(new CartridgeUiMessage(message));
    }
}
