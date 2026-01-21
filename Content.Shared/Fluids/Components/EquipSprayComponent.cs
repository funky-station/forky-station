// SPDX-FileCopyrightText: 2025 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Fluids.Components;

/// <summary>
/// Allows items with the spray component to be equipped and sprayable with a unique action.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class EquipSprayComponent : Component
{
    /// <summary>
    /// Verb locid that will come up when interacting with the sprayer. Set to null for no verb!
    /// </summary>
    [DataField]
    public LocId? VerbLocId;
}
