// SPDX-FileCopyrightText: 2026 mq
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Client.Weapons.Ranged.Systems;

namespace Content.Client.Weapons.Ranged.Components;

/// <summary>
/// Visualizer for gun mag presence; can change states based on ammo count or toggle visibility entirely.
/// </summary>
[RegisterComponent, Access(typeof(GunSystem))]
public sealed partial class MagazineVisualsComponent : Component
{
    /// <summary>
    /// What RsiState we use.
    /// </summary>
    [DataField]
    public string? MagState;

    /// <summary>
    /// How many steps there are
    /// </summary>
    [DataField("steps")]
    public int MagSteps;

    /// <summary>
    /// Should we hide when the count is 0
    /// </summary>
    [DataField]
    public bool ZeroVisible;
}

public enum GunVisualLayers : byte
{
    Base,
    BaseUnshaded,
    Mag,
    MagUnshaded,
}
