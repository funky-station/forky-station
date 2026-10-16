using Content.Shared._Funkystation.Inventory.Systems;

namespace Content.Shared._Funkystation.Inventory.Components;

/// <summary>
/// Component that allows an entity to supply an entity with <see cref="FunkyInventoryComponent"/> with slots
/// </summary>
[RegisterComponent, Access(typeof(FunkyInventorySupplierUtilitySystem))]
public sealed partial class FunkyInventorySupplierComponent : Component
{
    [DataField]
    public List<FunkyInventoryTemplate> Templates { get; private set; }

    public HashSet<EntityUid> Users { get; private set; }
}
