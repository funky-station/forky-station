// SPDX-FileCopyrightText: 2023 eoineoineoin
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Events;

/// <summary>
///     Remove order from database.
/// </summary>
[Serializable, NetSerializable]
public sealed class CargoConsoleRemoveOrderMessage : BoundUserInterfaceMessage
{
    public int OrderId;

    public CargoConsoleRemoveOrderMessage(int orderId)
    {
        OrderId = orderId;
    }
}
