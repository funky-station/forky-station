using Content.Shared._Funkystation.Inventory.Systems;

namespace Content.Shared._Funkystation.Inventory.Components;

/// <summary>
/// Component that gives an entity an inventory (hotbar, clothing, etc.)
/// </summary>
[RegisterComponent, Access(typeof(FunkyInventoryUtilitySystem))]
public sealed partial class FunkyInventoryComponent : Component
{
    [DataField(serverOnly: true)]
    public HashSet<(FunkyInventoryTemplate, EntityUid)> Templates { get; private set; }

    public List<FunkyInventorySlotEnum> Slots { get; private set; }
}
