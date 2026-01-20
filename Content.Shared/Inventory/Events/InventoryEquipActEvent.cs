// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Inventory.Events;

/// <summary>
/// This event is used to tell the server-inventorysystem someone wants to equip something
/// </summary>
[NetSerializable, Serializable]
public sealed class InventoryEquipActEvent : EntityEventArgs
{
    public readonly NetEntity Uid;
    public readonly NetEntity ItemUid;
    public readonly string Slot;
    public readonly bool Silent;
    public readonly bool Force;

    public InventoryEquipActEvent(NetEntity uid, NetEntity itemUid, string slot, bool silent = false, bool force = false)
    {
        Uid = uid;
        ItemUid = itemUid;
        Slot = slot;
        Silent = silent;
        Force = force;
    }
}
