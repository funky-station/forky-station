namespace Content.Shared._Funkystation.Traits.Unions;

[RegisterComponent]
public sealed partial class StationUnionsComponent : Component
{
    [DataField]
    public List<StationUnion> Unions { get; private set; } = new();
}

public sealed class StationUnion
{
    public string Name = string.Empty;
    public List<string> Departments = [];
    public Dictionary<EntityUid, UnionMemberInfo> Members = new();
    public EntityUid Leader = EntityUid.Invalid;
    public bool OnStrike;
    // list of all the unions that merged together to form this one
    public List<string> GroupingIds = [];
}

public sealed class UnionMemberInfo
{
    public bool EligibleForLeader;
    // original grouping the person joined under. i.e, Engineer -> Engineering, and if it gets merged, we preserve that.
    public string GroupingId = string.Empty;
    public List<UnionMemberNote> Notes = new();
}

public sealed class UnionMemberNote
{
    public string Title = string.Empty;
    public string Text = string.Empty;
    public string Author = string.Empty;
    public TimeSpan Time;
}
