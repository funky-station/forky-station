// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Inventory.Events;

[NetSerializable, Serializable]
public sealed class UseSlotNetworkMessage : EntityEventArgs
{
    // The slot-owner is implicitly the client that is sending this message.
    // Otherwise clients could start forcefully undressing other clients.
    public readonly string Slot;

    public UseSlotNetworkMessage(string slot)
    {
        Slot = slot;
    }
}
