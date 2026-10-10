using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Inventory;

[Prototype]
public sealed partial class FunkyInventoryTemplatePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField(required: true)]
    public List<FunkyInventorySlotEnum> Slots { get; private set; }
}
