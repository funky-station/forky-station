using Content.Shared._Funkystation.Inventory.Components;

namespace Content.Shared._Funkystation.Inventory.Systems;

public sealed class FunkyInventorySupplierUtilitySystem : EntitySystem
{
    /// <summary>
    /// Try to add a FunkyInventoryComponent to which this is equipped
    /// </summary>
    /// <returns><c>true</c> if a user was successfully added, <c>false</c> otherwise</returns>
    public static bool TryAddUser(Entity<FunkyInventorySupplierComponent> ent, Entity<FunkyInventoryComponent> user)
    {
        return ent.Comp.Users.Add(user);
    }

    public static void RemoveUser(Entity<FunkyInventorySupplierComponent> ent, EntityUid user)
    {
        ent.Comp.Users.Remove(user);
    }
}
