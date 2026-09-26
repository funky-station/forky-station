using Content.Client._AS.CanvasDesign.UI;
using Content.Client.Eui;
using Content.Shared._AS.CanvasDesign;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client._AS.CanvasDesign;

[UsedImplicitly]
public sealed class CanvasDesignHistoryEui : BaseEui
{
    private CanvasDesignHistoryWindow? _window;

    public override void Opened()
    {
        base.Opened();
        _window = new CanvasDesignHistoryWindow();
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
        _window.PreviewRequested += id => SendMessage(new CanvasDesignHistorySelectMessage(id));
        _window.OpenCentered();
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is CanvasDesignHistoryEuiState history)
        {
            _window?.SetHistory(history.Entries, history.Selected, history.ShowTargets);
        }
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);
        if (msg is CanvasDesignHistoryPreviewMessage preview)
        {
            _window?.SetPreview(preview.Preview);
        }
    }

    public override void Closed()
    {
        base.Closed();
        _window?.Close();
        _window = null;
    }
}
