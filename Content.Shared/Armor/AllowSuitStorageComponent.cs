// SPDX-FileCopyrightText: 2024 Errant
// SPDX-FileCopyrightText: 2024 Джексон Миссиссиппи
// SPDX-License-Identifier: MIT

using Content.Shared.Whitelist;

namespace Content.Shared.Armor;

/// <summary>
///     Used on outerclothing to allow use of suit storage
/// </summary>
[RegisterComponent]
public sealed partial class AllowSuitStorageComponent : Component
{
    /// <summary>
    /// Whitelist for what entities are allowed in the suit storage slot.
    /// </summary>
    [DataField]
    public EntityWhitelist Whitelist = new()
    {
        Components = new[] {"Item"}
    };
}
