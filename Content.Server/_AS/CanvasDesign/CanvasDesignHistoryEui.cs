using System.Linq;
using Content.Server.EUI;
using Content.Shared._AS.CanvasDesign;
using Content.Shared.Eui;

namespace Content.Server._AS.CanvasDesign;

public sealed class CanvasDesignHistoryEui(
    EntityUid? canvas,
    CanvasDesignSystem system,
    int? initialSelection = null) : BaseEui
{
    private int? _selected;

    public override void Opened()
    {
        base.Opened();
        var entries = GetEntries();
        _selected = initialSelection is { } requested && system.IsPreviewInHistory(canvas, requested)
            ? requested
            : entries.FirstOrDefault()?.PreviewId;
        StateDirty();
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);
        if (msg is not CanvasDesignHistorySelectMessage select ||
            !system.IsPreviewInHistory(canvas, select.PreviewId))
        {
            return;
        }

        _selected = select.PreviewId;
        if (system.TryGetPreview(select.PreviewId, out var preview))
        {
            SendMessage(new CanvasDesignHistoryPreviewMessage(CreatePreview(select.PreviewId, preview)));
        }
    }

    public override EuiStateBase GetNewState()
    {
        CanvasDesignPreviewData? selected = null;
        if (_selected is { } id && system.TryGetPreview(id, out var preview))
        {
            selected = CreatePreview(id, preview);
        }

        return new CanvasDesignHistoryEuiState(GetEntries(), selected, canvas == null);
    }

    private CanvasDesignHistoryEntry[] GetEntries()
    {
        return canvas is { } uid ? system.GetHistory(uid) : system.GetAllHistory();
    }

    private static CanvasDesignPreviewData CreatePreview(int id, CanvasDesignPreview preview)
    {
        return new CanvasDesignPreviewData(id,
            preview.Width,
            preview.Height,
            preview.Background,
            (uint[]) preview.Pixels.Clone(),
            preview.Name,
            preview.Description,
            preview.SavedBy,
            preview.SavedAt,
            preview.ServerOffsetMinutes);
    }
}
