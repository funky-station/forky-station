using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server._Funkystation.Nda;

[RegisterComponent]
public sealed partial class NdaCabinetComponent : Component
{
    [DataField]
    public bool Populated;

    [DataField(required: true)]
    public ProtoId<DepartmentPrototype> Department;

    [DataField(required: true)]
    public EntProtoId PaperPrototype;
}
