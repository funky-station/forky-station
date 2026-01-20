// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Inventory.Events;

[NetSerializable, Serializable]
public sealed class OpenSlotStorageNetworkMessage : EntityEventArgs
{
    public readonly string Slot;

    public OpenSlotStorageNetworkMessage(string slot)
    {
        Slot = slot;
    }
}
