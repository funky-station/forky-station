// SPDX-FileCopyrightText: 2025 Token <56667933+TokenStyle@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Clothing.Components;

/// <summary>
/// Handles the changes to ClothingComponent.EquippedPrefix when toggled.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ToggleClothingPrefixComponent : Component
{
    /// <summary>
    /// Clothing's EquippedPrefix when activated.
    /// </summary>
    [DataField]
    public string? PrefixOn = "on";

    /// <summary>
    /// Clothing's EquippedPrefix when deactivated.
    /// </summary>
    [DataField]
    public string? PrefixOff;
}
