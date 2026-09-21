using Content.Shared.Construction.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._Funkystation.ConstructionChalk;

[Prototype]
public sealed partial class ChalkCategoryPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = null!;

    [DataField(required: true)]
    public LocId Name { get; private set; }

    // which chalk mode this category shows up under
    [DataField(required: true)]
    public ChalkMode Mode { get; private set; }

    [DataField(required: true)]
    public SpriteSpecifier Icon { get; private set; } = null!;

    // entries shown in this category's sub-radial
    [DataField(required: true)]
    public List<ChalkEntry> Entries { get; private set; } = [];
}

[DataDefinition]
public sealed partial class ChalkEntry
{
    [DataField(required: true)]
    public LocId Name { get; private set; }

    [DataField(required: true)]
    public SpriteSpecifier Icon { get; private set; } = null!;

    // the ConstructionPrototype this entry places a mark for
    [DataField(required: true)]
    public ProtoId<ConstructionPrototype> ConstructionPrototype { get; private set; }
}
