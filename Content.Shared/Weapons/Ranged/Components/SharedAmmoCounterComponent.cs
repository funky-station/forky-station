// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Ranged.Components;

/// <summary>
/// Shows an ItemStatus with the ammo of the gun. Adjusts based on what the ammoprovider is.
/// </summary>
[NetworkedComponent]
public abstract partial class SharedAmmoCounterComponent : Component {}
