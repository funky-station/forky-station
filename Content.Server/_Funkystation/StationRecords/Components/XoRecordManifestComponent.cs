using Content.Server._Funkystation.Pager;
using Content.Server._Funkystation.StationRecords.Systems;
using Content.Server.CrewManifest;
using Content.Server.CriminalRecords.Systems;
using Content.Server.StationRecords.Systems;
using Content.Shared.StationRecords;

namespace Content.Server._Funkystation.StationRecords.Components;

[RegisterComponent, Access(
     typeof(XoRecordManifestSystem),
     typeof(XoRecordsConsoleSystem),
     typeof(GeneralStationRecordConsoleSystem),
     typeof(CrewManifestSystem),
     typeof(CriminalRecordsConsoleSystem),
     typeof(PagerSystem))]
public sealed partial class XoRecordManifestComponent : Component
{
    [DataField]
    public Dictionary<uint, GeneralStationRecord> Published = new();

    [DataField]
    public HashSet<uint> Discrepancies = [];

    [DataField]
    public uint NextManualId = uint.MaxValue;
}
