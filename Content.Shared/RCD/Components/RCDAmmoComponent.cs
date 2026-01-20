// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 chromiumboy
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-License-Identifier: MIT

using Content.Shared.RCD.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.RCD.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(RCDAmmoSystem))]
public sealed partial class RCDAmmoComponent : Component
{
    /// <summary>
    /// How many charges are contained in this ammo cartridge.
    /// Can be partially transferred into an RCD, until it is empty then it gets deleted.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Charges = 30;
}
