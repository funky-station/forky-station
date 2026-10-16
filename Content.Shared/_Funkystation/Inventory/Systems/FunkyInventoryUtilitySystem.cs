using Content.Shared._Funkystation.Inventory.Components;

namespace Content.Shared._Funkystation.Inventory.Systems;

public sealed class FunkyInventoryUtilitySystem : EntitySystem
{
    [Dependency] private EntityQuery<FunkyInventorySupplierComponent> _supplierQuery;

    /// <summary>
    /// Try to add the slots from a FunkyInventorySupplierComponent
    /// </summary>
    /// <param name="ent">The entity to which we add the inventory supplier</param>
    /// <param name="supplier">The entity with the FunkyInventorySupplier component</param>
    /// <returns><c>true</c> if ANY new inventories were added, <c>false</c> otherwise</returns>
    public bool TryAddInventorySupplier(Entity<FunkyInventoryComponent> ent, Entity<FunkyInventorySupplierComponent> supplier)
    {
        var added = false;
        foreach (var template in supplier.Comp.Templates)
        {
            added |= ent.Comp.Templates.Add((template, supplier));
        }

        if (added)
        {
            FunkyInventorySupplierUtilitySystem.TryAddUser(supplier, ent);
        }

        return added;
    }

    /// <summary>
    /// Remove all slots owned by <c>ent</c>
    /// </summary>
    /// <param name="ent">The entity from which we are removing the supplier</param>
    /// <param name="supplier">The entity whose slots to remove. Might not have FunkyInventorySupplierComponent anymore.</param>
    public void RemoveInventorySupplier(Entity<FunkyInventoryComponent> ent, Entity<FunkyInventorySupplierComponent> supplier)
    {
        foreach (var (template, owner) in ent.Comp.Templates)
        {
            if (owner != supplier.Owner)
                continue;
            ent.Comp.Templates.Remove((template, owner));
        }
        if (_supplierQuery.TryComp(supplier, out var comp))
        {
            FunkyInventorySupplierUtilitySystem.RemoveUser(supplier, ent.Owner);
        }
    }
}
