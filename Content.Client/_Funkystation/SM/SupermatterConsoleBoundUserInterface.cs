using Content.Shared._Funkystation.SM;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._Funkystation.SM;

[UsedImplicitly]
public sealed class SupermatterConsoleBoundUserInterface : BoundUserInterface
{
    private SupermatterConsoleWindow? _window;

    public SupermatterConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<SupermatterConsoleWindow>();
        _window.OnClose += Close;

        if (State is SupermatterConsoleBoundUserInterfaceState s)
            _window.UpdateState(s);
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is SupermatterConsoleBoundUserInterfaceState s)
            _window?.UpdateState(s);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _window?.Dispose();
    }
}
