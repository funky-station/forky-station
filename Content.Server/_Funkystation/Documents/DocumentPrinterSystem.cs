using System.Linq;
using Content.Server.Popups;
using Content.Server.Tools;
using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared._Funkystation.Documents;
using Content.Shared._Funkystation.Documents.Components;
using Content.Shared.Interaction;
using Content.Shared.Labels.Components;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.NameModifier.Components;
using Content.Shared.Paper;
using Content.Shared.Popups;
using Content.Shared.Wires;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.Documents;

/// <summary>
/// Handles document printer UI state, printing, and copying inserted paper
/// </summary>
public sealed partial class DocumentPrinterSystem : EntitySystem
{
    [Dependency] private IPrototypeManager _proto = null!;
    [Dependency] private PaperSystem _paper = null!;
    [Dependency] private AccessReaderSystem _accessReader = null!;
    [Dependency] private UserInterfaceSystem _ui = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private SharedAppearanceSystem _appearance = null!;
    [Dependency] private IGameTiming _timing = null!;
    [Dependency] private ItemSlotsSystem _itemSlots = null!;
    [Dependency] private LabelSystem _label = null!;
    [Dependency] private MetaDataSystem _metaData = null!;
    [Dependency] private IRobustRandom _random = null!;
    [Dependency] private ToolSystem _toolSystem = null!;
    [Dependency] private SharedDoAfterSystem _doAfter = null!;
    [Dependency] private PopupSystem _popup = null!;

    private const string PaperSlotId = "Paper";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DocumentPrinterComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<DocumentPrinterComponent, EntInsertedIntoContainerMessage>(OnSlotChanged);
        SubscribeLocalEvent<DocumentPrinterComponent, EntRemovedFromContainerMessage>(OnSlotChanged);

        SubscribeLocalEvent<DocumentPrinterComponent, BoundUIOpenedEvent>(OnUiOpened);
        SubscribeLocalEvent<DocumentPrinterComponent, DocumentPrinterPrintMessage>(OnPrintRequested);
        SubscribeLocalEvent<DocumentPrinterComponent, DocumentPrinterCopyMessage>(OnCopyRequested);
        SubscribeLocalEvent<DocumentPrinterComponent, DocumentPrinterEjectMessage>(OnEjectRequested);
        SubscribeLocalEvent<DocumentPrinterComponent, GotEmaggedEvent>(OnEmagged);

        SubscribeLocalEvent<DocumentPrinterComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<DocumentPrinterComponent, DocumentPrinterJamClearDoAfterEvent>(OnJamClearDoAfter);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<DocumentPrinterComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (!_ui.IsUiOpen(uid, DocumentPrinterUiKey.Key))
                continue;

            if (comp.NextPrintTime != TimeSpan.Zero && _timing.CurTime >= comp.NextPrintTime)
            {
                comp.NextPrintTime = TimeSpan.Zero;

                foreach (var actor in _ui.GetActors(uid, DocumentPrinterUiKey.Key))
                {
                    UpdateUiState(uid, comp, actor);
                }
            }
        }
    }

    private void OnComponentInit(EntityUid uid, DocumentPrinterComponent comp, ComponentInit args)
    {
        if (!HasComp<ItemSlotsComponent>(uid))
            return;

        if (_itemSlots.TryGetSlot(uid, PaperSlotId, out var slot))
            comp.PaperSlot = slot;
    }

    private void OnSlotChanged(EntityUid uid, DocumentPrinterComponent comp, ContainerModifiedMessage args)
    {
        if (args.Container.ID != comp.PaperSlot.ID)
            return;

        UpdateUiState(uid, comp, uid);
    }

    private void OnUiOpened(EntityUid uid, DocumentPrinterComponent comp, BoundUIOpenedEvent args)
    {
        UpdateUiState(uid, comp, args.Actor);
    }

    private void OnEmagged(EntityUid uid, DocumentPrinterComponent comp, ref GotEmaggedEvent args)
    {
        if (comp.EmagDocuments.Count == 0)
            return;

        args.Handled = true;
        UpdateUiState(uid, comp, args.UserUid);
    }

    /// <summary>
    /// Every document ID currently printable on this printer
    /// </summary>
    private IEnumerable<string> GetActiveDocumentIds(EntityUid uid, DocumentPrinterComponent comp)
    {
        foreach (var id in comp.AvailableDocuments)
        {
            yield return id;
        }

        if (HasComp<EmaggedComponent>(uid))
        {
            foreach (var id in comp.EmagDocuments)
            {
                yield return id;
            }
        }

        if (comp.ManagerWireCut)
        {
            foreach (var id in comp.ManagerDocuments)
            {
                yield return id;
            }
        }
    }

    /// <summary>
    /// Rebuilds and pushes the full document list for this printer
    /// </summary>
    private void UpdateUiState(EntityUid uid, DocumentPrinterComponent comp, EntityUid actor)
    {
        var grouped = new Dictionary<string, List<DocumentEntry>>();

        foreach (var docId in GetActiveDocumentIds(uid, comp))
        {
            if (!_proto.TryIndex<DocumentPrototype>(docId, out var doc))
                continue;

            var accessible = IsDocAccessible(comp, actor, doc);

            if (!grouped.TryGetValue(doc.Category, out var list))
                grouped[doc.Category] = list = new List<DocumentEntry>();

            list.Add(new DocumentEntry(
                doc.ID,
                Loc.GetString(doc.Name),
                Loc.GetString(doc.Description),
                accessible,
                doc.RequiredAccess ?? new List<string>()));
        }

        var paperEntity = comp.PaperSlot.Item;
        var isPaperInserted = paperEntity is { } inserted && HasComp<PaperComponent>(inserted);

        string? paperName = null;
        if (isPaperInserted && paperEntity is { } paper)
        {
            TryComp<NameModifierComponent>(paper, out var nameMod);
            paperName = nameMod?.BaseName ?? MetaData(paper).EntityName;
        }

        var canPrint = !comp.Jammed && _timing.CurTime >= comp.NextPrintTime;
        var canCopy = isPaperInserted && canPrint;

        _ui.SetUiState(uid,
            DocumentPrinterUiKey.Key,
            new DocumentPrinterBoundUserInterfaceState(grouped, isPaperInserted, paperName, canCopy, canPrint, comp.Jammed));
    }

    /// <summary>
    /// Checks a document's RequiredAccess against the player's access
    /// </summary>
    private bool IsDocAccessible(DocumentPrinterComponent comp, EntityUid actor, DocumentPrototype doc)
    {
        if (comp.AccessBroken)
            return true;

        if (doc.RequiredAccess is not { Count: > 0 })
            return true;

        var tags = _accessReader.FindAccessTags(actor);
        return doc.RequiredAccess.Any(s => tags.Contains(s));
    }

    /// <summary>
    /// Jam roll, called once per completed print/copy job
    /// </summary>
    private bool RollJam(DocumentPrinterComponent comp)
    {
        return comp.JamOneInChance > 0 && _random.Next(comp.JamOneInChance) == 0;
    }

    private void TriggerJam(EntityUid uid, DocumentPrinterComponent comp, EntityUid actor)
    {
        comp.Jammed = true;
        _appearance.SetData(uid, DocumentPrinterVisuals.VisualState, DocumentPrinterVisualState.Jammed);
        _audio.PlayPvs(comp.JamSound, uid);
        _popup.PopupEntity(Loc.GetString("document-printer-jam-occurred"), uid, PopupType.LargeCaution);
        UpdateUiState(uid, comp, actor);
    }

    private void OnPrintRequested(EntityUid uid, DocumentPrinterComponent comp, DocumentPrinterPrintMessage msg)
    {
        var actor = msg.Actor;

        if (comp.Jammed || _timing.CurTime < comp.NextPrintTime)
            return;

        if (!GetActiveDocumentIds(uid, comp).Contains(msg.DocumentId))
            return;

        if (!_proto.TryIndex<DocumentPrototype>(msg.DocumentId, out var doc))
            return;

        if (!IsDocAccessible(comp, actor, doc))
            return;

        comp.NextPrintTime = _timing.CurTime + comp.PrintCooldown;

        _appearance.SetData(uid, DocumentPrinterVisuals.VisualState, DocumentPrinterVisualState.Printing);
        _audio.PlayPvs(comp.PrintSound, uid);

        var coords = Transform(uid).Coordinates;
        var printDelay = comp.PrintDelay;

        Timer.Spawn(printDelay,
            () =>
        {
            if (!Exists(uid))
                return;

            if (RollJam(comp))
            {
                TriggerJam(uid, comp, actor);
                return;
            }

            var paper = Spawn(doc.PaperPrototype, coords);
            _paper.SetContent(paper, Loc.GetString(doc.Content));

            _appearance.SetData(uid, DocumentPrinterVisuals.VisualState, DocumentPrinterVisualState.Normal);
        });

        UpdateUiState(uid, comp, actor);
    }

    /// <summary>
    /// Duplicates whatever paper is in the copy slot
    /// </summary>
    private void OnCopyRequested(EntityUid uid, DocumentPrinterComponent comp, DocumentPrinterCopyMessage msg)
    {
        var actor = msg.Actor;

        if (comp.Jammed || _timing.CurTime < comp.NextPrintTime)
            return;

        var source = comp.PaperSlot.Item;
        if (source is not { } sourceUid)
            return;

        if (!TryComp<PaperComponent>(sourceUid, out var sourcePaper))
            return;

        if (!TryComp(sourceUid, out MetaDataComponent? sourceMeta))
            return;

        TryComp<LabelComponent>(sourceUid, out var sourceLabel);
        TryComp<NameModifierComponent>(sourceUid, out var sourceNameMod);

        var content = sourcePaper.Content;
        var stampState = sourcePaper.StampState;
        var stampedBy = sourcePaper.StampedBy;
        var locked = sourcePaper.EditingDisabled;
        var name = sourceNameMod?.BaseName ?? sourceMeta.EntityName;
        var label = sourceLabel?.CurrentLabel;
        var protoId = sourceMeta.EntityPrototype?.ID ?? comp.CopyPaperId.ToString();

        comp.NextPrintTime = _timing.CurTime + comp.PrintCooldown;

        _appearance.SetData(uid, DocumentPrinterVisuals.VisualState, DocumentPrinterVisualState.Printing);
        _audio.PlayPvs(comp.PrintSound, uid);

        var coords = Transform(uid).Coordinates;
        var printDelay = comp.PrintDelay;

        Timer.Spawn(printDelay,
            () =>
        {
            if (!Exists(uid))
                return;

            if (RollJam(comp))
            {
                TriggerJam(uid, comp, actor);
                return;
            }

            var printed = Spawn(protoId, coords);

            if (TryComp<PaperComponent>(printed, out var newPaper))
            {
                _paper.SetContent((printed, newPaper), content);

                if (stampState != null)
                {
                    foreach (var stamp in stampedBy)
                    {
                        _paper.TryStamp((printed, newPaper), stamp, stampState);
                    }
                }

                newPaper.EditingDisabled = locked;
            }

            _metaData.SetEntityName(printed, name);

            if (label is { } l)
                _label.Label(printed, l);

            _appearance.SetData(uid, DocumentPrinterVisuals.VisualState, DocumentPrinterVisualState.Normal);
        });

        UpdateUiState(uid, comp, actor);
    }

    private void OnEjectRequested(EntityUid uid, DocumentPrinterComponent comp, DocumentPrinterEjectMessage msg)
    {
        _itemSlots.TryEjectToHands(uid, comp.PaperSlot, msg.Actor);
    }

    public void RefreshUi(EntityUid uid)
    {
        if (TryComp<DocumentPrinterComponent>(uid, out var comp))
            UpdateUiState(uid, comp, uid);
    }

    private void OnInteractUsing(EntityUid uid, DocumentPrinterComponent comp, InteractUsingEvent args)
    {
        if (args.Handled || !comp.Jammed)
            return;

        if (!TryComp<WiresPanelComponent>(uid, out var panel) || !panel.Open)
        {
            _popup.PopupEntity(Loc.GetString("document-printer-jam-panel-closed"), uid, args.User);
            return;
        }

        if (!_toolSystem.HasQuality(args.Used, comp.JamClearToolQuality))
            return;

        var doAfterArgs = new DoAfterArgs(EntityManager, args.User, comp.JamClearDelay, new DocumentPrinterJamClearDoAfterEvent(), uid, target: uid, used: args.Used)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = true,
            BreakOnHandChange = true,
        };

        if (_doAfter.TryStartDoAfter(doAfterArgs))
        {
            comp.JamLoopSoundEntity = _audio.PlayPvs(comp.JamLoopSound, uid, AudioParams.Default.WithLoop(true))?.Entity;
            args.Handled = true;
        }
    }

    private void OnJamClearDoAfter(EntityUid uid, DocumentPrinterComponent comp, DocumentPrinterJamClearDoAfterEvent args)
    {
        StopJamLoopSound(comp);

        if (args.Cancelled || args.Handled || !comp.Jammed)
            return;

        comp.Jammed = false;
        _appearance.SetData(uid, DocumentPrinterVisuals.VisualState, DocumentPrinterVisualState.Normal);
        _popup.PopupEntity(Loc.GetString("document-printer-jam-cleared"), uid);
        _audio.PlayPvs(comp.PrintSound, uid);

        args.Handled = true;
        UpdateUiState(uid, comp, args.User);
    }

    private void StopJamLoopSound(DocumentPrinterComponent comp)
    {
        if (comp.JamLoopSoundEntity is { } soundEnt)
        {
            _audio.Stop(soundEnt);
            comp.JamLoopSoundEntity = null;
        }
    }
}
