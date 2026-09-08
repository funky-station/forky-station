using Content.Server.GameTicking;
using Content.Shared._Funkystation.Handwriting;
using Content.Shared.Paper;
using Content.Shared.Roles;
using Content.Shared.Station;
using Content.Shared.StationRecords;
using Content.Shared.StationRecords.Components;
using Content.Shared.StationRecords.Systems;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Prototypes;

namespace Content.Server._Funkystation.Nda;

public sealed partial class NdaCabinetSystem : EntitySystem
{
    [Dependency] private StationRecordsSystem _stationRecords = default!;
    [Dependency] private SharedStationSystem _station = default!;
    [Dependency] private SharedStorageSystem _storage = default!;
    [Dependency] private PaperSystem _paper = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    
    private const string EmployeeNamePlaceholder = "{EMPLOYEE_NAME}";
    private const string EmployeeSignaturePlaceholder = "{EMPLOYEE_SIGNATURE}";
    private const string CentComStampState = "paper_stamp-centcom";
    
    // i got lazy
    private static readonly StampDisplayInfo CentComStamp = new()
    {
        StampedName = "stamp-component-stamped-name-centcom",
        StampedColor = Color.FromHex("#006600"),
    };

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GameRunLevelChangedEvent>(OnRunLevelChanged);
    }

    private void OnRunLevelChanged(GameRunLevelChangedEvent ev)
    {
        if (ev.New != GameRunLevel.InRound)
            return;

        var query = EntityQueryEnumerator<NdaCabinetComponent, StorageComponent>();
        while (query.MoveNext(out var uid, out var cabinet, out var storage))
        {
            if (cabinet.Populated)
                continue;

            cabinet.Populated = true;

            var station = _station.GetOwningStation(uid);
            if (station is not { } stationUid || !TryComp<StationRecordsComponent>(stationUid, out var records))
                continue;
            
            // this works so :famine:
            foreach (var (_, record) in _stationRecords.GetRecordsOfType<GeneralStationRecord>((stationUid, records)))
            {
                SpawnNdaInCabinet(uid, cabinet, storage, record);
            }
        }
    }

    private void SpawnNdaInCabinet(EntityUid cabinetUid, NdaCabinetComponent cabinet, StorageComponent storage, GeneralStationRecord record)
    {
        if (!_proto.TryIndex<JobPrototype>(record.JobPrototype, out var job) || job.RequiresNda != cabinet.Department)
            return;

        var paper = Spawn(cabinet.PaperPrototype, Transform(cabinetUid).Coordinates);
        var paperComp = Comp<PaperComponent>(paper);

        var signature = HandwritingFontHelper.WrapIfHandwritten(EntityManager, EntityUid.Invalid, record.Name);
        var content = paperComp.Content
            .Replace(EmployeeNamePlaceholder, record.Name)
            .Replace(EmployeeSignaturePlaceholder, signature);
        _paper.SetContent((paper, paperComp), content);

        _paper.TryStamp((paper, paperComp), CentComStamp, CentComStampState);

        _storage.Insert(cabinetUid, paper, out _, storageComp: storage, playSound: false);
    }
}
