using System.Linq;
using Content.Server._Funkystation.StationRecords.Components; // funky
using Content.Server._Funkystation.StationRecords.Systems; // funky
using Content.Server.Station.Systems;
using Content.Server.StationRecords.Components;
using Content.Shared.StationRecords;
using Robust.Server.GameObjects;

namespace Content.Server.StationRecords.Systems;

public sealed partial class GeneralStationRecordConsoleSystem : EntitySystem
{
    [Dependency] private UserInterfaceSystem _ui = default!;
    [Dependency] private StationSystem _station = default!;
    [Dependency] private StationRecordsSystem _stationRecords = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<XoRecordManifestSystem.XoRecordManifestUpdatedEvent>(OnManifestUpdated); // funky

        Subs.BuiEvents<GeneralStationRecordConsoleComponent>(GeneralStationRecordConsoleKey.Key, subs =>
        {
            subs.Event<BoundUIOpenedEvent>(UpdateUserInterface);
            subs.Event<SelectStationRecord>(OnKeySelected);
            subs.Event<SetStationRecordFilter>(OnFiltersChanged);
            subs.Event<DeleteStationRecord>(OnRecordDelete);
        });
    }

    // funky start, broadcast
    private void OnManifestUpdated(XoRecordManifestSystem.XoRecordManifestUpdatedEvent args)
    {
        var query = EntityQueryEnumerator<GeneralStationRecordConsoleComponent>();
        while (query.MoveNext(out var uid, out var console))
        {
            if (_station.GetOwningStation(uid) == args.Station)
                UpdateUserInterface((uid, console));
        }
    }
    // funky end

    private void OnRecordDelete(Entity<GeneralStationRecordConsoleComponent> ent, ref DeleteStationRecord args)
    {
        if (!ent.Comp.CanDeleteEntries)
            return;

        var owning = _station.GetOwningStation(ent.Owner);

        if (owning != null)
            _stationRecords.RemoveRecord(new StationRecordKey(args.Id, owning.Value));
        UpdateUserInterface(ent); // Apparently an event does not get raised for this.
    }

    private void UpdateUserInterface<T>(Entity<GeneralStationRecordConsoleComponent> ent, ref T args)
    {
        UpdateUserInterface(ent);
    }

    // TODO: instead of copy paste shitcode for each record console, have a shared records console comp they all use
    // then have this somehow play nicely with creating ui state
    // if that gets done put it in StationRecordsSystem console helpers section :)
    private void OnKeySelected(Entity<GeneralStationRecordConsoleComponent> ent, ref SelectStationRecord msg)
    {
        ent.Comp.ActiveKey = msg.SelectedKey;
        UpdateUserInterface(ent);
    }

    private void OnFiltersChanged(Entity<GeneralStationRecordConsoleComponent> ent, ref SetStationRecordFilter msg)
    {
        if (ent.Comp.Filter == null ||
            ent.Comp.Filter.Type != msg.Type || ent.Comp.Filter.Value != msg.Value)
        {
            ent.Comp.Filter = new StationRecordsFilter(msg.Type, msg.Value);
            UpdateUserInterface(ent);
        }
    }

    private void UpdateUserInterface(Entity<GeneralStationRecordConsoleComponent> ent)
    {
        var (uid, console) = ent;
        var owningStation = _station.GetOwningStation(uid);

        // funky start
        if (owningStation is not { } station || !HasComp<StationRecordsComponent>(station))
        {
            _ui.SetUiState(uid, GeneralStationRecordConsoleKey.Key, new GeneralStationRecordConsoleState());
            return;
        }

        if (!TryComp<XoRecordManifestComponent>(station, out var manifest))
        {
            _ui.SetUiState(uid, GeneralStationRecordConsoleKey.Key, new GeneralStationRecordConsoleState());
            return;
        }
        var listing = new Dictionary<uint, string>();
        foreach (var (id, record) in manifest.Published)
        {
            if (_stationRecords.IsSkipped(console.Filter, record))
                continue;

            listing.Add(id, record.Name);
        }
        // Funky end

        switch (listing.Count)
        {
            case 0:
                _ui.SetUiState(uid, GeneralStationRecordConsoleKey.Key, new GeneralStationRecordConsoleState());
                return;
            default:
                if (console.ActiveKey == null)
                    console.ActiveKey = listing.Keys.First();
                break;
        }

        if (console.ActiveKey is not { } id2) // funky changed to id2, record2
            return;

        manifest.Published.TryGetValue(id2, out var record2); // funky

        GeneralStationRecordConsoleState newState = new(id2, record2, listing, console.Filter, ent.Comp.CanDeleteEntries); // funky
        _ui.SetUiState(uid, GeneralStationRecordConsoleKey.Key, newState);
    }
}
