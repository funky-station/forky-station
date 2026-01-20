// SPDX-FileCopyrightText: 2025 pathetic meowmeow
// SPDX-FileCopyrightText: 2025 ScarKy0
// SPDX-FileCopyrightText: 2025 Nemanja
// SPDX-License-Identifier: MIT

using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared.Cargo.Components;

/// <summary>
/// Makes a sellable object portion out its value to a specified department rather than the station default
/// </summary>
[RegisterComponent]
public sealed partial class OverrideSellComponent : Component
{
    /// <summary>
    /// The account that will receive the primary funds from this being sold.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<CargoAccountPrototype> OverrideAccount;
}
