using Content.Shared._Funkystation.FirelockBolt;
using JetBrains.Annotations;

namespace Content.Client._Funkystation.FirelockBolt;

[UsedImplicitly]
public sealed class FirelockOverrideBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private FirelockOverrideWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = new FirelockOverrideWindow();
        _window.OnClose += Close;
        _window.SetOverride += SendOverride;
        _window.OpenCentered();
    }

    private void SendOverride(bool value)
    {
        SendMessage(new FirelockOverrideSetMessage(value));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is FirelockOverrideBuiState firelockState)
            _window?.UpdateState(firelockState);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;

        _window?.Close();
        _window = null;
    }
}
