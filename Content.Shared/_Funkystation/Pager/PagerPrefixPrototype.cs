using Content.Shared.Access;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Pager;

// maps a department to the first two digits of its pager numbers
[Prototype]
public sealed partial class PagerPrefixPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField(required: true)]
    public ProtoId<DepartmentPrototype> Department;

    [DataField(required: true)]
    public int Prefix;

    // any of these on the inserted id lets the pda app mass page this prefix
    [DataField]
    public List<ProtoId<AccessLevelPrototype>> MassPageAccess = [];
}
