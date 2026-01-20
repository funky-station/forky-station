// SPDX-FileCopyrightText: 2026 mq
// SPDX-FileCopyrightText: 2024 PoorMansDreams
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Client.Weapons.Ranged.Systems;

namespace Content.Client.Weapons.Ranged.Components;

[RegisterComponent, Access(typeof(GunSystem))]
public sealed partial class SpentAmmoVisualsComponent : Component
{
    /// <summary>
    /// Should we do "{_state}-spent" or just "spent"
    /// </summary>
    [DataField]
    public bool Suffix = true;

    [DataField]
    public string State = "base";
}

public enum AmmoVisualLayers : byte
{
    Base,
    Tip,
}
