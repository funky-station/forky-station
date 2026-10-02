using Content.Shared._Funkystation.Inventory;

namespace Content.Shared.Inventory.Events;

public sealed class EquipableInventoryChangeEvent(bool add, EquipableInventoryComponent inventory) : EntityEventArgs
{
    public readonly bool Add = add;
    public readonly EquipableInventoryComponent Inventory = inventory;
}
