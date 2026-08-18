using Content.Server._Funkystation.StationRecords.Components;
using Content.Server.Station.Systems;
using Content.Shared._Funkystation.CCVar;
using Content.Shared._Funkystation.Pager;
using Content.Shared._Funkystation.StationRecords;
using Content.Shared.StationRecords;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;

namespace Content.Server._Funkystation.StationRecords.Systems;

public sealed partial class XoRecordsConsoleSystem : EntitySystem
{
    [Dependency] private UserInterfaceSystem _ui = null!;
    [Dependency] private StationSystem _station = null!;
    [Dependency] private XoRecordManifestSystem _manifest = null!;
    [Dependency] private IConfigurationManager _cfg = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XoRecordManifestSystem.XoRecordManifestUpdatedEvent>(OnManifestUpdated);

        Subs.BuiEvents<XoRecordsConsoleComponent>(XoRecordsConsoleKey.Key,
            subs =>
        {
            subs.Event<BoundUIOpenedEvent>(OnBuiOpened);
            subs.Event<XoSelectRecordMessage>(OnSelectRecord);
            subs.Event<XoSubmitRecordMessage>(OnSubmitRecord);
            subs.Event<XoVerifyRecordMessage>(OnVerifyRecord);
            subs.Event<XoCreateRecordMessage>(OnCreateRecord);
            subs.Event<XoDeleteRecordMessage>(OnDeleteRecord);
        });
    }

    private void OnBuiOpened(Entity<XoRecordsConsoleComponent> ent, ref BoundUIOpenedEvent args)
    {
        UpdateUserInterface(ent);
    }

    private void OnManifestUpdated(XoRecordManifestSystem.XoRecordManifestUpdatedEvent args)
    {
        var query = EntityQueryEnumerator<XoRecordsConsoleComponent>();
        while (query.MoveNext(out var uid, out var console))
        {
            if (_station.GetOwningStation(uid) == args.Station)
                UpdateUserInterface((uid, console));
        }
    }

    private void OnSelectRecord(Entity<XoRecordsConsoleComponent> ent, ref XoSelectRecordMessage msg)
    {
        ent.Comp.ActiveKey = msg.SelectedKey;
        UpdateUserInterface(ent);
    }

    private void OnVerifyRecord(Entity<XoRecordsConsoleComponent> ent, ref XoVerifyRecordMessage msg)
    {
        if (!_cfg.GetCVar(XoRecordsCVars.ManualRecordsEnabled))
            return;

        var owning = _station.GetOwningStation(ent.Owner);
        if (owning == null)
            return;

        _manifest.RunDiscrepancyCheck(owning.Value);
    }

    private void OnCreateRecord(Entity<XoRecordsConsoleComponent> ent, ref XoCreateRecordMessage msg)
    {
        if (!_cfg.GetCVar(XoRecordsCVars.ManualRecordsEnabled))
            return;

        var owning = _station.GetOwningStation(ent.Owner);
        if (owning == null)
            return;

        var newId = _manifest.CreateManualRecord(owning.Value);
        if (newId != 0)
            ent.Comp.ActiveKey = newId;
    }

    private void OnDeleteRecord(Entity<XoRecordsConsoleComponent> ent, ref XoDeleteRecordMessage msg)
    {
        if (!_cfg.GetCVar(XoRecordsCVars.ManualRecordsEnabled))
            return;

        var owning = _station.GetOwningStation(ent.Owner);
        if (owning == null)
            return;

        if (ent.Comp.ActiveKey == msg.Id)
            ent.Comp.ActiveKey = null;

        _manifest.DeleteRecord(owning.Value, msg.Id);
    }

    private void OnSubmitRecord(Entity<XoRecordsConsoleComponent> ent, ref XoSubmitRecordMessage msg)
    {
        if (!_cfg.GetCVar(XoRecordsCVars.ManualRecordsEnabled))
            return;

        var owning = _station.GetOwningStation(ent.Owner);
        if (owning == null)
            return;

        var name = msg.Fields.Name.Trim();
        if (name.Length > 100)
            name = name[..100];

        var jobTitle = msg.Fields.JobTitle.Trim();
        if (jobTitle.Length > 100)
            jobTitle = jobTitle[..100];

        var species = msg.Fields.Species.Trim();
        if (species.Length > 100)
            species = species[..100];

        if (name.Length == 0 || jobTitle.Length == 0 || species.Length == 0)
            return;

        var fingerprint = string.IsNullOrWhiteSpace(msg.Fields.Fingerprint) ? null : msg.Fields.Fingerprint.Trim();
        if (fingerprint is { Length: > 100 })
            fingerprint = fingerprint[..100];

        var dna = string.IsNullOrWhiteSpace(msg.Fields.Dna) ? null : msg.Fields.Dna.Trim();
        if (dna is { Length: > 100 })
            dna = dna[..100];

        var age = Math.Clamp(msg.Fields.Age, 18, 120);

        int? pagerNumber = msg.Fields.PagerNumber;
        if (pagerNumber is { } pVal && !SharedPagerSystem.IsValidNumber(pVal))
            pagerNumber = null;

        _manifest.TrySubmitRecord(owning.Value, msg.Id, name, age, jobTitle, species, msg.Fields.Gender, fingerprint, dna, pagerNumber);
    }

    private void UpdateUserInterface(Entity<XoRecordsConsoleComponent> ent)
    {
        var (uid, console) = ent;
        var owning = _station.GetOwningStation(uid);
        var isEditable = _cfg.GetCVar(XoRecordsCVars.ManualRecordsEnabled);

        if (owning == null || !HasComp<StationRecordsComponent>(owning))
        {
            _ui.SetUiState(uid, XoRecordsConsoleKey.Key, new XoRecordsConsoleState(new List<XoRecordListingEntry>(), null, false, null, isEditable, 0));
            return;
        }

        var manifest = EnsureComp<XoRecordManifestComponent>(owning.Value);

        var listing = new List<XoRecordListingEntry>();
        foreach (var (id, published) in manifest.Published)
        {
            var flagged = _manifest.IsFlagged(owning.Value, id, manifest);
            listing.Add(new XoRecordListingEntry(id, published.Name, flagged));
        }

        XoRecordFields? selectedFields = null;
        var selectedFlagged = false;

        if (console.ActiveKey is { } id2)
        {
            selectedFlagged = _manifest.IsFlagged(owning.Value, id2, manifest);
            if (manifest.Published.TryGetValue(id2, out var selectedPublished))
            {
                selectedFields = new XoRecordFields(
                    selectedPublished.Name,
                    selectedPublished.Age,
                    selectedPublished.JobTitle,
                    selectedPublished.Species,
                    selectedPublished.Gender,
                    selectedPublished.Fingerprint,
                    selectedPublished.DNA,
                    selectedPublished.PagerNumber);
            }
        }

        var discrepancyCount = _manifest.GetDiscrepancyCount(owning.Value, manifest);
        var newState = new XoRecordsConsoleState(listing, console.ActiveKey, selectedFlagged, selectedFields, isEditable, discrepancyCount);
        _ui.SetUiState(uid, XoRecordsConsoleKey.Key, newState);
    }
}
