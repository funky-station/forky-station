// SPDX-FileCopyrightText: 2024 Magnus Larsen
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

namespace Content.Client.Weapons.Melee.Components;

/// <summary>
/// Used for melee attack animations. Typically just has a fadeout.
/// </summary>
[RegisterComponent]
public sealed partial class WeaponArcVisualsComponent : Component
{
    public EntityUid? User;

    [DataField("animation")]
    public WeaponArcAnimation Animation = WeaponArcAnimation.None;

    [ViewVariables(VVAccess.ReadWrite), DataField("fadeOut")]
    public bool Fadeout = true;
}

public enum WeaponArcAnimation : byte
{
    None,
    Thrust,
    Slash,
}
