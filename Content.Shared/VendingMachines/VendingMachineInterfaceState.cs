// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2022 Andreas Kämper
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines
{
    [Serializable, NetSerializable]
    public sealed class VendingMachineEjectMessage : BoundUserInterfaceMessage
    {
        public readonly InventoryType Type;
        public readonly string ID;
        public VendingMachineEjectMessage(InventoryType type, string id)
        {
            Type = type;
            ID = id;
        }
    }

    [Serializable, NetSerializable]
    public enum VendingMachineUiKey
    {
        Key,
    }
}
