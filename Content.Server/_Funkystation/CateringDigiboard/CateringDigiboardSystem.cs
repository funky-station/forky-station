using System.Linq;
using System.Text;
using Content.Server.Popups;
using Content.Shared._Funkystation.CateringDigiboard;
using Content.Shared._Funkystation.CateringDigiboard.Components;
using Content.Shared.Paper;
using Content.Shared.Storage.EntitySystems;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.CateringDigiboard;

/// <summary>
/// handles UI state, menu editing, and printing menus to its own storage
/// </summary>
public sealed partial class CateringDigiboardSystem : EntitySystem
{
    [Dependency] private UserInterfaceSystem _ui = null!;
    [Dependency] private PaperSystem _paper = null!;
    [Dependency] private SharedStorageSystem _storage = null!;
    [Dependency] private PopupSystem _popup = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private IGameTiming _timing = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CateringDigiboardComponent, BoundUIOpenedEvent>(OnUiOpened);
        SubscribeLocalEvent<CateringDigiboardComponent, CateringDigiboardAddItemMessage>(OnAddItem);
        SubscribeLocalEvent<CateringDigiboardComponent, CateringDigiboardAddCategoryMessage>(OnAddCategory);
        SubscribeLocalEvent<CateringDigiboardComponent, CateringDigiboardEditItemMessage>(OnEditItem);
        SubscribeLocalEvent<CateringDigiboardComponent, CateringDigiboardRemoveItemMessage>(OnRemoveItem);
        SubscribeLocalEvent<CateringDigiboardComponent, CateringDigiboardReorderMessage>(OnReorderItem);
        SubscribeLocalEvent<CateringDigiboardComponent, CateringDigiboardPrintMessage>(OnPrint);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<CateringDigiboardComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.PendingCopies <= 0 || _timing.CurTime < comp.NextPrintTime)
                continue;

            PrintOneCopy(uid, comp);
            comp.PendingCopies--;

            if (comp.PendingCopies > 0)
            {
                comp.NextPrintTime = _timing.CurTime + comp.PrintInterval;
                _audio.PlayPvs(comp.PrintSound, uid);
            }
            else
            {
                if (comp.PendingDropped > 0)
                    _popup.PopupEntity(Loc.GetString("catering-digiboard-menus-dropped", ("count", comp.PendingDropped)), uid, comp.PendingActor);

                comp.PendingDropped = 0;
                comp.PendingContent = string.Empty;
            }

            UpdateUiState(uid, comp);
        }
    }

    private void OnUiOpened(EntityUid uid, CateringDigiboardComponent comp, BoundUIOpenedEvent args)
    {
        UpdateUiState(uid, comp);
    }

    private void UpdateUiState(EntityUid uid, CateringDigiboardComponent comp)
    {
        _ui.SetUiState(uid,
            CateringDigiboardUiKey.Key,
            new CateringDigiboardBoundUserInterfaceState(comp.Items, comp.MaxItems, comp.PendingCopies > 0));
    }

    private void OnAddItem(EntityUid uid, CateringDigiboardComponent comp, CateringDigiboardAddItemMessage args)
    {
        if (comp.Items.Count(i => !i.IsCategory) >= comp.MaxItems)
        {
            _popup.PopupEntity(Loc.GetString("catering-digiboard-menu-full"), uid, args.Actor);
            return;
        }

        var id = (comp.NextItemId++).ToString();
        comp.Items.Add(new CateringMenuItem(id, Loc.GetString("catering-digiboard-default-item-name"), string.Empty, 0));
        UpdateUiState(uid, comp);
    }

    private void OnAddCategory(EntityUid uid, CateringDigiboardComponent comp, CateringDigiboardAddCategoryMessage args)
    {
        var id = (comp.NextItemId++).ToString();
        comp.Items.Add(new CateringMenuItem(id, Loc.GetString("catering-digiboard-default-category-name"), string.Empty, 0, true));
        UpdateUiState(uid, comp);
    }

    private void OnEditItem(EntityUid uid, CateringDigiboardComponent comp, CateringDigiboardEditItemMessage args)
    {
        var item = comp.Items.FirstOrDefault(i => i.Id == args.ItemId);
        if (item is null)
            return;

        item.Name = args.Name;

        if (!item.IsCategory)
        {
            item.Description = args.Description;
            item.ScripCost = Math.Max(0, args.ScripCost);
        }

        UpdateUiState(uid, comp);
    }

    private void OnRemoveItem(EntityUid uid, CateringDigiboardComponent comp, CateringDigiboardRemoveItemMessage args)
    {
        comp.Items.RemoveAll(i => i.Id == args.ItemId);
        UpdateUiState(uid, comp);
    }

    private void OnReorderItem(EntityUid uid, CateringDigiboardComponent comp, CateringDigiboardReorderMessage args)
    {
        var index = comp.Items.FindIndex(i => i.Id == args.ItemId);
        if (index == -1)
            return;

        var item = comp.Items[index];
        comp.Items.RemoveAt(index);

        var target = Math.Clamp(args.NewIndex, 0, comp.Items.Count);
        comp.Items.Insert(target, item);

        UpdateUiState(uid, comp);
    }

    private void OnPrint(EntityUid uid, CateringDigiboardComponent comp, CateringDigiboardPrintMessage args)
    {
        if (comp.PendingCopies > 0)
        {
            _popup.PopupEntity(Loc.GetString("catering-digiboard-still-printing"), uid, args.Actor);
            return;
        }

        if (comp.Items.Count == 0)
        {
            _popup.PopupEntity(Loc.GetString("catering-digiboard-menu-empty"), uid, args.Actor);
            return;
        }

        var count = args.Count switch
        {
            <= 1 => 1,
            <= 5 => 5,
            _ => 10
        };

        comp.PendingContent = FormatMenu(comp);
        comp.PendingActor = args.Actor;
        comp.PendingDropped = 0;
        comp.PendingCopies = count;
        comp.NextPrintTime = _timing.CurTime + comp.PrintInterval;

        _audio.PlayPvs(comp.PrintSound, uid);

        UpdateUiState(uid, comp);
    }

    private void PrintOneCopy(EntityUid uid, CateringDigiboardComponent comp)
    {
        var coords = Transform(uid).Coordinates;
        var paper = Spawn(comp.MenuPaperId, coords);

        if (TryComp<PaperComponent>(paper, out var paperComp))
            _paper.SetContent((paper, paperComp), comp.PendingContent);

        if (!_storage.Insert(uid, paper, out _, comp.PendingActor))
            comp.PendingDropped++;
    }

    private string FormatMenu(CateringDigiboardComponent comp)
    {
        var sb = new StringBuilder();

        foreach (var item in comp.Items)
        {
            if (item.IsCategory)
            {
                var headerLine = $"── {item.Name} ──";
                var padded = CenterPad(headerLine, comp.MenuLineWidth);
                sb.AppendLine(Loc.GetString("catering-digiboard-category-header", ("line", padded)));
                sb.AppendLine();
                continue;
            }

            sb.AppendLine(Loc.GetString("catering-digiboard-menu-item-line", ("name", item.Name), ("cost", item.ScripCost)));

            if (!string.IsNullOrWhiteSpace(item.Description))
                sb.AppendLine(Loc.GetString("catering-digiboard-menu-item-description", ("description", item.Description)));

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    // this is my best attempt at centering LOL
    private static string CenterPad(string line, int width)
    {
        var pad = (width - line.Length) / 2;
        return pad > 0 ? new string(' ', pad) + line : line;
    }
}
