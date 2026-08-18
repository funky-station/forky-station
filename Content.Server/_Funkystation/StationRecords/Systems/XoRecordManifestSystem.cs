using Content.Server._Funkystation.StationRecords.Components;
using Content.Server.GameTicking;
using Content.Server.StationRecords.Systems;
using Content.Shared._Funkystation.CCVar;
using Content.Shared.StationRecords;
using Content.Shared.CriminalRecords;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;

namespace Content.Server._Funkystation.StationRecords.Systems;

public sealed partial class XoRecordManifestSystem : EntitySystem
{
    [Dependency] private StationRecordsSystem _stationRecords = null!;
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private IPrototypeManager _prototypeManager = null!;

    private bool _manualEnabled;

    public sealed class XoRecordManifestUpdatedEvent(EntityUid station) : EntityEventArgs
    {
        public readonly EntityUid Station = station;
    }

    public override void Initialize()
    {
        base.Initialize();

        Subs.CVar(_cfg, XoRecordsCVars.ManualRecordsEnabled, OnCVarChanged, true);

        SubscribeLocalEvent<GameRunLevelChangedEvent>(OnRunLevelChanged);
        SubscribeLocalEvent<AfterGeneralRecordCreatedEvent>(OnRecordCreated);
        SubscribeLocalEvent<RecordModifiedEvent>(OnRecordModified);
        SubscribeLocalEvent<RecordRemovedEvent>(OnRecordRemoved);
    }

    private void OnCVarChanged(bool enabled)
    {
        _manualEnabled = enabled;

        if (_manualEnabled)
            return;

        var query = EntityQueryEnumerator<XoRecordManifestComponent, StationRecordsComponent>();
        while (query.MoveNext(out var uid, out var manifest, out var records))
        {
            manifest.Published.Clear();
            foreach (var (id, record) in _stationRecords.GetRecordsOfType<GeneralStationRecord>(uid, records))
            {
                manifest.Published[id] = record with { };
            }

            manifest.Discrepancies.Clear();
            RaiseLocalEvent(new XoRecordManifestUpdatedEvent(uid));
        }
    }

    private void OnRunLevelChanged(GameRunLevelChangedEvent ev)
    {
        if (ev.New != GameRunLevel.InRound)
            return;

        var query = EntityQueryEnumerator<StationRecordsComponent>();
        while (query.MoveNext(out var station, out var records))
        {
            var manifest = EnsureComp<XoRecordManifestComponent>(station);

            foreach (var (id, record) in _stationRecords.GetRecordsOfType<GeneralStationRecord>(station, records))
            {
                manifest.Published[id] = record with { };
            }

            manifest.Discrepancies.Clear();
            RaiseLocalEvent(new XoRecordManifestUpdatedEvent(station));
        }
    }

    private void OnRecordCreated(AfterGeneralRecordCreatedEvent ev)
    {
        var manifest = EnsureComp<XoRecordManifestComponent>(ev.Key.OriginStation);

        if (!_stationRecords.TryGetRecord<GeneralStationRecord>(ev.Key, out var record))
            return;

        manifest.Published[ev.Key.Id] = record with { };
        RaiseLocalEvent(new XoRecordManifestUpdatedEvent(ev.Key.OriginStation));
    }

    private void OnRecordModified(RecordModifiedEvent ev)
    {
        if (_manualEnabled)
            return;

        if (!TryComp<XoRecordManifestComponent>(ev.Key.OriginStation, out var manifest) ||
            !_stationRecords.TryGetRecord<GeneralStationRecord>(ev.Key, out var record))
            return;

        manifest.Published[ev.Key.Id] = record with { };
        RaiseLocalEvent(new XoRecordManifestUpdatedEvent(ev.Key.OriginStation));
    }

    private void OnRecordRemoved(RecordRemovedEvent ev)
    {
        if (!TryComp<XoRecordManifestComponent>(ev.Station, out var manifest))
            return;

        var changed = manifest.Published.Remove(ev.Key.Id);
        changed |= manifest.Discrepancies.Remove(ev.Key.Id);

        if (changed)
            RaiseLocalEvent(new XoRecordManifestUpdatedEvent(ev.Station));
    }

    public void RunDiscrepancyCheck(EntityUid station, XoRecordManifestComponent? manifest = null, StationRecordsComponent? records = null)
    {
        if (!_manualEnabled)
            return;

        if (!Resolve(station, ref manifest, ref records, false))
            return;

        var flagged = new HashSet<uint>();

        foreach (var (id, published) in manifest.Published)
        {
            if (!_stationRecords.TryGetRecord<GeneralStationRecord>(new StationRecordKey(id, station),
                    out var live,
                    records))
                continue;

            if (Discrepant(published, live))
                flagged.Add(id);
        }

        manifest.Discrepancies = flagged;
        RaiseLocalEvent(new XoRecordManifestUpdatedEvent(station));
    }

    private static bool Discrepant(GeneralStationRecord published, GeneralStationRecord live)
    {
        return published.Name != live.Name
            || published.Age != live.Age
            || published.JobTitle != live.JobTitle
            || published.Species != live.Species
            || published.Gender != live.Gender
            || published.Fingerprint != live.Fingerprint
            || published.DNA != live.DNA
            || published.PagerNumber != live.PagerNumber;
    }

    public bool IsFlagged(EntityUid station, uint id, XoRecordManifestComponent? manifest = null)
    {
        return Resolve(station, ref manifest, false) && manifest.Discrepancies.Contains(id);
    }

    public int GetDiscrepancyCount(EntityUid station, XoRecordManifestComponent? manifest = null)
    {
        return Resolve(station, ref manifest, false) ? manifest.Discrepancies.Count : 0;
    }

    public bool TryGetPublished(EntityUid station, uint id, out GeneralStationRecord? published, XoRecordManifestComponent? manifest = null)
    {
        published = null;

        if (!Resolve(station, ref manifest, false))
            return false;

        if (!manifest.Published.TryGetValue(id, out var record))
            return false;

        published = record;
        return true;
    }

    public uint CreateManualRecord(EntityUid station, XoRecordManifestComponent? manifest = null)
    {
        if (!_manualEnabled || !Resolve(station, ref manifest))
            return 0;

        var id = manifest.NextManualId--;
        var newRecord = new GeneralStationRecord()
        {
            Name = "New Record",
            Age = 30,
            JobTitle = "Visitor",
            Species = "Human",
            Gender = Gender.Epicene
        };

        manifest.Published[id] = newRecord;

        var key = new StationRecordKey(id, station);
        _stationRecords.AddRecordEntry(key, newRecord with { });
        _stationRecords.AddRecordEntry(key, new CriminalRecord());

        RaiseLocalEvent(new XoRecordManifestUpdatedEvent(station));
        return id;
    }

    public void DeleteRecord(EntityUid station, uint id, XoRecordManifestComponent? manifest = null)
    {
        if (!_manualEnabled || !Resolve(station, ref manifest))
            return;

        var changed = manifest.Published.Remove(id);
        changed |= manifest.Discrepancies.Remove(id);
        _stationRecords.RemoveRecord(new StationRecordKey(id, station));

        if (changed)
            RaiseLocalEvent(new XoRecordManifestUpdatedEvent(station));
    }

    public bool TrySubmitRecord(EntityUid station,
        uint id,
        string name,
        int age,
        string jobTitle,
        string species,
        Gender gender,
        string? fingerprint,
        string? dna,
        int? pagerNumber,
        XoRecordManifestComponent? manifest = null)
    {
        if (!_manualEnabled || !Resolve(station, ref manifest))
            return false;

        GeneralStationRecord baseRecord;
        if (manifest.Published.TryGetValue(id, out var existing))
            baseRecord = existing with { };
        else if (_stationRecords.TryGetRecord<GeneralStationRecord>(new StationRecordKey(id, station), out var live))
            baseRecord = live with { };
        else
            return false;

        var published = baseRecord with
        {
            Name = name,
            Age = age,
            JobTitle = jobTitle,
            Species = species,
            Gender = gender,
            Fingerprint = fingerprint,
            DNA = dna,
            PagerNumber = pagerNumber,
        };

        foreach (var job in _prototypeManager.EnumeratePrototypes<JobPrototype>())
        {
            if (!Loc.GetString(job.Name).Equals(jobTitle, StringComparison.OrdinalIgnoreCase))
                continue;

            published.JobPrototype = job.ID;
            published.JobIcon = job.Icon;
            break;
        }

        manifest.Published[id] = published;
        manifest.Discrepancies.Remove(id);

        // if this is a manually created record, keep the live station record in sync
        if (id >= 1_000_000_000)
        {
            var key = new StationRecordKey(id, station);
            _stationRecords.AddRecordEntry(key, published with { });
        }

        RaiseLocalEvent(new XoRecordManifestUpdatedEvent(station));
        return true;
    }
}
