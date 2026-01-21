// SPDX-FileCopyrightText: 2025 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Hitscan.Components;

/// <summary>
/// Hitscan entities that have this component will deal stamina damage to the target.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HitscanStaminaDamageComponent : Component
{
    /// <summary>
    /// How much stamania damage the hitscan weapon will do when hitting a target.
    /// </summary>
    [DataField]
    public float StaminaDamage = 10.0f;
}
