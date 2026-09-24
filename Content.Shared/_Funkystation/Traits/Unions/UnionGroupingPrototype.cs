using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Traits.Unions;

[Prototype]
public sealed partial class UnionGroupingPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// dept that the grouping represents
    [DataField(required: true)]
    public string Department = string.Empty;

    [DataField]
    public char? DisplayInitial;

    /// if a union can merge into other unions (false for sec, as an example)
    [DataField]
    public bool CanMerge = true;

    /// if union does not have MinMembers, merge with closest union
    [DataField]
    public int MinMembers = 1;

    /// ordered list of unions to merge with.
    [DataField]
    public List<string> MergesWith = new();

    /// dept-themed stuff to give for a leader. if null, uses defaults
    [DataField]
    public ProtoId<UnionRoleItemSetPrototype>? LeaderItemSet;
}
