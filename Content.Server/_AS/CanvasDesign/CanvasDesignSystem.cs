using Content.Server.Administration.Logs;
using Content.Server.Administration;
using Content.Server.EUI;
using Content.Server.Popups;
using Content.Shared._AS.CanvasDesign;
using Content.Shared.Administration;
using Content.Server._AS.EditableMetadata;
using Content.Shared._AS.EditableMetadata;
using Content.Shared.Database;
using Content.Shared.Verbs;
using Content.Shared.Popups;
using Content.Shared.Paper;
using Content.Shared.UserInterface;
using Content.Shared.Wires;
using Robust.Server.GameObjects;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System.Linq;
using Content.Server.Administration.Managers;
using Robust.Shared.Player;

namespace Content.Server._AS.CanvasDesign;

/// <summary>
/// Server authority for editable pixel canvases. Initializes component data and UI state,
/// controls editor access, validates and rate-limits saves, applies metadata, writes admin
/// logs, and retains bounded snapshots for administrative previews.
/// </summary>
public sealed partial class CanvasDesignSystem : EntitySystem
{
    /// <summary>Maximum number of admin-preview snapshots retained for the current process.</summary>
    private const int MaxStoredPreviews = 10_000; // Roughly 10 MB of memory for 100x100 canvases.
    private const int MaxHistoryPerCanvas = 100;
    [Dependency] private IAdminLogManager _adminLog = null!;
    [Dependency] private IAdminManager _adminManager = null!;
    [Dependency] private EuiManager _eui = null!;
    [Dependency] private IGameTiming _timing = null!;
    [Dependency] private ASEditableMetadataSystem _editableMetadata = null!;
    [Dependency] private UserInterfaceSystem _ui = null!;
    [Dependency] private PopupSystem _popup = null!;

    private readonly Dictionary<EntityUid, TimeSpan> _nextSaveAllowed = new();
    private readonly Dictionary<EntityUid, (int Width, int Height)> _knownDimensions = new();
    private readonly Dictionary<int, CanvasDesignPreview> _previews = new();
    private readonly Queue<int> _previewOrder = new();
    private readonly Dictionary<EntityUid, Queue<int>> _history = new();
    private int _nextPreviewId = 1;

    public override void Initialize()
    {
        SubscribeLocalEvent<CanvasDesignComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<CanvasDesignComponent, CanvasDesignSaveMessage>(OnSave);
        SubscribeLocalEvent<CanvasDesignComponent, EntityTerminatingEvent>(OnTerminating);
        SubscribeLocalEvent<CanvasDesignComponent, GetVerbsEvent<InteractionVerb>>(OnGetVerbs);
        SubscribeLocalEvent<CanvasDesignComponent, GetVerbsEvent<Verb>>(OnGetAdminVerbs);
        SubscribeLocalEvent<CanvasDesignComponent, ActivatableUIOpenAttemptEvent>(OnOpenAttempt);
        SubscribeLocalEvent<CanvasDesignComponent, BoundUIOpenedEvent>(OnUiOpened);
    }

    private void OnGetAdminVerbs(Entity<CanvasDesignComponent> ent, ref GetVerbsEvent<Verb> args)
    {
        if (!TryComp<ActorComponent>(args.User, out var actor) ||
            !_adminManager.HasAdminFlag(actor.PlayerSession, AdminFlags.Logs))
        {
            return;
        }

        args.Verbs.Add(new Verb
        {
            Text = Loc.GetString("canvas-design-verb-history"),
            Category = VerbCategory.Admin,
            Impact = LogImpact.Low,
            Act = () => _eui.OpenEui(new CanvasDesignHistoryEui(ent.Owner, this), actor.PlayerSession)
        });
    }

    private void OnMapInit(Entity<CanvasDesignComponent> ent, ref MapInitEvent args)
    {
        EnsureInitialized(ent);
        _knownDimensions[ent.Owner] = (ent.Comp.Width, ent.Comp.Height);
        var ui = EnsureComp<UserInterfaceComponent>(ent);
        _ui.SetUi((ent.Owner, ui), CanvasDesignUiKey.Key, new InterfaceData("CanvasDesignBoundUserInterface"));
        UpdateUi(ent);
    }

    private void OnGetVerbs(Entity<CanvasDesignComponent> ent, ref GetVerbsEvent<InteractionVerb> args)
    {
        if (!ent.Comp.AddEditorVerb || IsLocked(ent.Owner) || !args.CanInteract || !args.CanAccess || args.Hands == null)
            return;

        var user = args.User;
        args.Verbs.Add(new InteractionVerb
        {
            Text = Loc.GetString("canvas-design-verb-edit"),
            IconEntity = GetNetEntity(ent.Owner),
            Priority = 2,
            Act = () => TryOpenEditor(ent, user)
        });
    }

    private void TryOpenEditor(Entity<CanvasDesignComponent> ent, EntityUid user)
    {
        if (IsLocked(ent.Owner))
        {
            ShowLocked(ent.Owner, user);
            return;
        }

        ReconcileDimensions(ent);

        if (!PanelRequirementMet(ent))
            return;

        if (IsBeingEdited(ent, user))
        {
            ShowEditorBusy(ent.Owner, user);
            return;
        }
        _ui.OpenUi(ent.Owner, CanvasDesignUiKey.Key, user);
    }

    private void OnOpenAttempt(Entity<CanvasDesignComponent> ent, ref ActivatableUIOpenAttemptEvent args)
    {
        if (IsLocked(ent.Owner))
        {
            args.Cancel();
            if (!args.Silent)
                ShowLocked(ent.Owner, args.User);
            return;
        }

        ReconcileDimensions(ent);

        if (!PanelRequirementMet(ent))
        {
            args.Cancel();
            return;
        }

        if (!IsBeingEdited(ent, args.User))
            return;
        args.Cancel();
        if (!args.Silent)
            ShowEditorBusy(ent.Owner, args.User);
    }

    private void OnUiOpened(Entity<CanvasDesignComponent> ent, ref BoundUIOpenedEvent args)
    {
        if (IsLocked(ent.Owner))
        {
            _ui.CloseUi(ent.Owner, CanvasDesignUiKey.Key, args.Actor);
            ShowLocked(ent.Owner, args.Actor);
            return;
        }

        ReconcileDimensions(ent);

        // This is a second guard for callers that bypass ActivatableUIOpenAttemptEvent.
        if (!Equals(args.UiKey, CanvasDesignUiKey.Key) || !IsBeingEdited(ent, args.Actor))
            return;

        _ui.CloseUi(ent.Owner, CanvasDesignUiKey.Key, args.Actor);
        ShowEditorBusy(ent.Owner, args.Actor); // If another player is editing, don't let other people also open it
    }

    private bool IsBeingEdited(Entity<CanvasDesignComponent> ent, EntityUid user)
    {
        return _ui.GetActors(ent.Owner, CanvasDesignUiKey.Key).Any(actor => actor != user);
    }

    private void ShowEditorBusy(EntityUid canvas, EntityUid user)
    {
        _popup.PopupEntity(Loc.GetString("canvas-design-editor-busy"), canvas, user, PopupType.SmallCaution);
    }

    /// <summary>
    /// Clamps prototype-controlled limits and creates a correctly sized background-filled canvas.
    /// </summary>
    public void EnsureInitialized(Entity<CanvasDesignComponent> ent)
    {
        ent.Comp.Width = Math.Clamp(ent.Comp.Width, 1, CanvasDesignComponent.MaxWidth);
        ent.Comp.Height = Math.Clamp(ent.Comp.Height, 1, CanvasDesignComponent.MaxHeight);
        ent.Comp.SaveCooldown = ent.Comp.SaveCooldown < TimeSpan.Zero
            ? TimeSpan.Zero
            : ent.Comp.SaveCooldown > TimeSpan.FromMinutes(1)
                ? TimeSpan.FromMinutes(1)
                : ent.Comp.SaveCooldown;

        if (ent.Comp.Pixels.Length != ent.Comp.PixelCount)
        {
            ent.Comp.Pixels = new uint[ent.Comp.PixelCount];
            Array.Fill(ent.Comp.Pixels, ent.Comp.PackedBackground);
        }
        Dirty(ent);
    }

    private bool IsLocked(EntityUid uid)
    {
        return TryComp<CanvasDesignLockComponent>(uid, out var lockComponent) && lockComponent.Locked;
    }

    private void ShowLocked(EntityUid canvas, EntityUid user)
    {
        _popup.PopupEntity(Loc.GetString("canvas-design-editor-locked"), canvas, user, PopupType.SmallCaution);
    }

    /// <summary>Permanently prevents further edits to a lockable canvas.</summary>
    public void Lock(Entity<CanvasDesignLockComponent> ent)
    {
        if (ent.Comp.Locked)
            return;

        ent.Comp.Locked = true;
        Dirty(ent);
        _ui.CloseUi(ent.Owner, CanvasDesignUiKey.Key);
    }

    private void ResizeCanvas(Entity<CanvasDesignComponent> ent, int oldWidth, int oldHeight)
    {
        var newWidth = Math.Clamp(ent.Comp.Width, 1, CanvasDesignComponent.MaxWidth);
        var newHeight = Math.Clamp(ent.Comp.Height, 1, CanvasDesignComponent.MaxHeight);
        var resized = new uint[newWidth * newHeight];
        Array.Fill(resized, ent.Comp.PackedBackground);

        if (oldWidth > 0 && oldHeight > 0 && ent.Comp.Pixels.Length == oldWidth * oldHeight)
        {
            var copyWidth = Math.Min(oldWidth, newWidth);
            var copyHeight = Math.Min(oldHeight, newHeight);
            for (var y = 0; y < copyHeight; y++)
            {
                Array.Copy(ent.Comp.Pixels, y * oldWidth, resized, y * newWidth, copyWidth);
            }
        }

        ent.Comp.Width = newWidth;
        ent.Comp.Height = newHeight;
        ent.Comp.Pixels = resized;
        _knownDimensions[ent.Owner] = (newWidth, newHeight);
        Dirty(ent);
        UpdateUi(ent);
    }

    private void ReconcileDimensions(Entity<CanvasDesignComponent> ent)
    {
        if (!_knownDimensions.TryGetValue(ent.Owner, out var known))
        {
            _knownDimensions[ent.Owner] = (ent.Comp.Width, ent.Comp.Height);
            return;
        }

        if (known.Width != ent.Comp.Width || known.Height != ent.Comp.Height)
            ResizeCanvas(ent, known.Width, known.Height);
    }

    private void OnSave(Entity<CanvasDesignComponent> ent, ref CanvasDesignSaveMessage args)
    {
        // Never accept a save from a client that does not currently own an open editor UI.
        if (!_ui.IsUiOpen(ent.Owner, CanvasDesignUiKey.Key, args.Actor))
            return;

        if (IsLocked(ent.Owner))
        {
            ShowLocked(ent.Owner, args.Actor);
            return;
        }

        if (!PanelRequirementMet(ent))
            return;

        var editAttempt = new CanvasDesignEditAttemptEvent(args.Actor);
        RaiseLocalEvent(ent.Owner, ref editAttempt);
        TryComp<ASEditableMetadataComponent>(ent, out var editable);
        if (editAttempt.Cancelled || !ChangesAreValid(ent.Comp, args.Changes) ||
            editable != null && (args.Name.Length > editable.MaxNameLength ||
                args.Description.Length > editable.MaxDescriptionLength ||
                !FormattedMessage.ValidMarkup(args.Description)))
            return;

        if (_nextSaveAllowed.TryGetValue(ent.Owner, out var nextAllowed) && _timing.CurTime < nextAllowed)
        {
            var seconds = Math.Max(1, (int) Math.Ceiling((nextAllowed - _timing.CurTime).TotalSeconds));
            _popup.PopupEntity(Loc.GetString("canvas-design-save-cooldown", ("seconds", seconds)),
                ent.Owner,
                args.Actor,
                PopupType.SmallCaution);
            return;
        }
        _nextSaveAllowed[ent.Owner] = _timing.CurTime + ent.Comp.SaveCooldown;

        var name = editable != null ? args.Name.Trim() : string.Empty;
        var description = editable != null ? args.Description.Trim() : string.Empty;
        var nameChanged = editable != null && editable.CustomName != name;
        var descriptionChanged = editable != null && editable.Description != description;
        var changed = ApplyChanges(ent, args.Changes);
        if (changed == 0 && !nameChanged && !descriptionChanged)
            return;

        var prototype = Prototype(ent);
        var entityMetadata = MetaData(ent);
        var defaultName = prototype?.Name ?? entityMetadata.EntityName;
        var defaultDescription = prototype?.Description ?? entityMetadata.EntityDescription;
        if (editable != null)
        {
            editable.CustomName = name;
            editable.Description = description;
            _editableMetadata.Apply((ent.Owner, editable), entityMetadata);
            Dirty(ent.Owner, editable);
        }
        var effectiveName = editable != null && !string.IsNullOrEmpty(name) ? name : defaultName;
        var effectiveDescription = editable != null && !string.IsNullOrEmpty(description)
            ? description
            : defaultDescription;
        Dirty(ent);
        UpdateUi(ent);

        var savedBy = TryComp<ActorComponent>(args.Actor, out var actor)
            ? actor.PlayerSession.Name
            : ToPrettyString(args.Actor);
        var previewId = StorePreview(ent.Owner, ent.Comp, effectiveName, effectiveDescription, savedBy, ent.Owner.Id, effectiveName);
        var loggedName = FormattedMessage.EscapeText(effectiveName);
        var loggedDescription = FormattedMessage.EscapeText(effectiveDescription.Replace('\r', ' ').Replace('\n', ' '));
        var metadataLog = editable != null
            ? $" and set the name to \"{loggedName}\" and description to \"{loggedDescription}\""
            : string.Empty;
        _adminLog.Add(LogType.CanvasDesign,
            LogImpact.Medium,
            $"{ToPrettyString(args.Actor):actor} changed {changed} pixels{metadataLog} on {ToPrettyString(ent.Owner):target}. Preview code: {previewId}");
        _popup.PopupEntity(Loc.GetString("canvas-design-save-success"), ent.Owner, args.Actor);
    }

    private static bool ChangesAreValid(CanvasDesignComponent component, CanvasPixelChange[]? changes)
    {
        if (!CanvasDesignComponent.DimensionsWithinLimit(component.Width, component.Height) ||
            component.Pixels.Length != component.PixelCount || changes == null || changes.Length > component.PixelCount)
            return false;
        // A bounded stack allocation rejects duplicate indices without allocating attacker-sized data.
        Span<bool> indices = stackalloc bool[component.PixelCount];
        foreach (var change in changes)
        {
            if (change.Index >= component.PixelCount || indices[change.Index])
                return false;
            indices[change.Index] = true;
        }
        return true;
    }

    private bool PanelRequirementMet(Entity<CanvasDesignComponent> ent)
    {
        return !ent.Comp.RequireOpenPanel ||
               TryComp<WiresPanelComponent>(ent, out var panel) && panel.Open;
    }

    private int ApplyChanges(Entity<CanvasDesignComponent> ent, CanvasPixelChange[] changes)
    {
        var count = 0;
        foreach (var change in changes)
        {
            // Only the exact configured background may retain transparency.
            // Every user-selected drawing color is normalized to fully opaque.
            var pixel = change.Color == ent.Comp.PackedBackground
                ? ent.Comp.PackedBackground
                : change.Color | 0xFF000000;
            if (ent.Comp.Pixels[change.Index] == pixel)
                continue;
            ent.Comp.Pixels[change.Index] = pixel;
            count++;
        }
        return count;
    }

    private void UpdateUi(Entity<CanvasDesignComponent> ent)
    {
        var metadata = MetaData(ent);
        var prototype = Prototype(ent);
        var hasEditableMetadata = TryComp<ASEditableMetadataComponent>(ent, out var editable);
        _ui.SetUiState(ent.Owner,
            CanvasDesignUiKey.Key,
            new CanvasDesignUiState(
            ent.Comp.Width,
            ent.Comp.Height,
            ent.Comp.PackedBackground,
            ent.Comp.PackedDefaultDrawingColor,
            hasEditableMetadata,
            editable?.MaxNameLength ?? 0,
            editable?.MaxDescriptionLength ?? 0,
            ent.Comp.EditorTitle,
            (uint[]) ent.Comp.Pixels.Clone(),
            editable?.CustomName ?? string.Empty,
            editable?.Description ?? string.Empty,
            prototype?.Name ?? metadata.EntityName,
            prototype?.Description ?? metadata.EntityDescription));
    }

    private int StorePreview(EntityUid canvasUid, CanvasDesignComponent canvas, string name, string description, string savedBy, int entityId, string entityName)
    {
        var id = _nextPreviewId++;
        if (_nextPreviewId <= 0)
            _nextPreviewId = 1;
        _previews[id] = new CanvasDesignPreview(canvas.Width,
            canvas.Height,
            canvas.PackedBackground,
            (uint[]) canvas.Pixels.Clone(),
            name,
            description,
            savedBy,
            entityId,
            entityName,
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            (int) DateTimeOffset.Now.Offset.TotalMinutes);
        _previewOrder.Enqueue(id);
        if (!_history.TryGetValue(canvasUid, out var history))
        {
            _history[canvasUid] = history = new Queue<int>();
        }
        history.Enqueue(id);
        while (history.Count > MaxHistoryPerCanvas)
        {
            history.Dequeue();
        }
        // snapshots are killed in order of oldest to newest
        while (_previewOrder.Count > MaxStoredPreviews)
        {
            _previews.Remove(_previewOrder.Dequeue());
        }
        return id;
    }

    /// <summary>Gets an admin-preview snapshot by its process-local preview code.</summary>
    public bool TryGetPreview(int id, out CanvasDesignPreview preview)
    {
        return _previews.TryGetValue(id, out preview!);
    }

    public CanvasDesignHistoryEntry[] GetHistory(EntityUid uid)
    {
        if (!_history.TryGetValue(uid, out var history))
        {
            return [];
        }

        return history
            .Reverse()
            .Where(_previews.ContainsKey)
            .Select(id => ToHistoryEntry(id, _previews[id]))
            .ToArray();
    }

    public CanvasDesignHistoryEntry[] GetAllHistory()
    {
        return _previewOrder
            .Reverse()
            .Where(_previews.ContainsKey)
            .Select(id => ToHistoryEntry(id, _previews[id]))
            .ToArray();
    }

    public bool IsPreviewInHistory(EntityUid? uid, int previewId)
    {
        if (!_previews.ContainsKey(previewId))
            return false;

        return uid == null ||
               _history.TryGetValue(uid.Value, out var history) && history.Contains(previewId);
    }

    private static CanvasDesignHistoryEntry ToHistoryEntry(int id, CanvasDesignPreview preview)
    {
        return new CanvasDesignHistoryEntry(id, preview.SavedBy, preview.EntityId, preview.EntityName);
    }

    private void OnTerminating(Entity<CanvasDesignComponent> ent, ref EntityTerminatingEvent args)
    {
        _nextSaveAllowed.Remove(ent.Owner);
        _knownDimensions.Remove(ent.Owner);
        _history.Remove(ent.Owner);
    }
}

/// <summary>Immutable server-side snapshot referenced by an admin-log preview code.</summary>
public sealed record CanvasDesignPreview(
    int Width,
    int Height,
    uint Background,
    uint[] Pixels,
    string Name,
    string Description,
    string SavedBy,
    int EntityId,
    string EntityName,
    long SavedAt,
    int ServerOffsetMinutes);
