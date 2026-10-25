using Content.Client._AS.CanvasDesign.UI;
using Content.Client._AS.CanvasDesign;
using Content.Shared._AS.CanvasDesign;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client._AS.CanvasDesign;

[UsedImplicitly]
public sealed class CanvasDesignBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private CanvasDesignEditorWindow? _window;

    protected override void Open()
    {
        base.Open();
        _window = this.CreateWindow<CanvasDesignEditorWindow>();
        var system = EntMan.System<CanvasDesignSystem>();
        _window.SaveRequested += (changes, name, description) =>
        {
            SendMessage(new CanvasDesignSaveMessage(changes, name, description));
            system.ClearDraft(Owner);
        };
        _window.DraftChanged += (pixels, name, description) =>
        {
            system.SetDraft(Owner, pixels, name, description);
            var preview = new CanvasDesignLocalPreviewEvent(pixels);
            EntMan.EventBus.RaiseLocalEvent(Owner, ref preview);
        };
        _window.DraftDiscarded += () => system.ClearDraft(Owner);
        _window.OnClose += () =>
        {
            var ended = new CanvasDesignLocalPreviewEndedEvent();
            EntMan.EventBus.RaiseLocalEvent(Owner, ref ended);
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not CanvasDesignUiState signState || _window == null)
            return;

        if (EntMan.System<CanvasDesignSystem>().TryGetDraft(Owner, out var draft))
        {
            _window.ApplyDraft(draft.Pixels,
                signState.Pixels,
                signState.Width,
                signState.Height,
                signState.Background,
                signState.DefaultDrawingColor,
                signState.MetadataEnabled,
                signState.MaxNameLength,
                signState.MaxDescriptionLength,
                signState.EditorTitle,
                draft.Name,
                draft.Description,
                signState.DefaultName,
                signState.DefaultDescription);
            var preview = new CanvasDesignLocalPreviewEvent(draft.Pixels);
            EntMan.EventBus.RaiseLocalEvent(Owner, ref preview);
            return;
        }

        _window.SetState(signState.Width,
            signState.Height,
            signState.Background,
            signState.DefaultDrawingColor,
            signState.MetadataEnabled,
            signState.MaxNameLength,
            signState.MaxDescriptionLength,
            signState.EditorTitle,
            signState.Pixels,
            signState.Name,
            signState.Description,
            signState.DefaultName,
            signState.DefaultDescription);
    }
}
