using Content.Server._Funkystation.StationRecords.Systems;

namespace Content.Server._Funkystation.StationRecords.Components;

[RegisterComponent, Access(typeof(XoRecordsConsoleSystem))]
public sealed partial class XoRecordsConsoleComponent : Component
{
    [DataField]
    public uint? ActiveKey;
}
