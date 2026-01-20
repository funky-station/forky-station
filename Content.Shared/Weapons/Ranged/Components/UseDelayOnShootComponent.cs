// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Ranged.Components;

/// <summary>
/// Applies UseDelay whenever the entity shoots.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(UseDelayOnShootSystem))]
public sealed partial class UseDelayOnShootComponent : Component
{

}
