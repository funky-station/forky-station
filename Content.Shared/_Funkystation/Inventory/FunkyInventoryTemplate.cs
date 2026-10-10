using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Inventory;

[DataDefinition]
public sealed partial class FunkyInventoryTemplate
{
    [DataField("name", required: true)]
    public ProtoId<FunkyInventoryTemplatePrototype> Prototype { get; private set; }

    public readonly Guid Guid = Guid.NewGuid();
}
