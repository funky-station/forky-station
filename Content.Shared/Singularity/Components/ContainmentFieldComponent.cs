// SPDX-FileCopyrightText: 2024 mhamster
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Singularity.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class ContainmentFieldComponent : Component
{
    /// <summary>
    /// The throw force for the field if an entity collides with it
    /// The lighter the mass the further it will throw. 5 mass will go about 4 tiles out, 70 mass goes only a couple tiles.
    /// </summary>
    [DataField("throwForce")]
    public float ThrowForce = 100f;

    /// <summary>
    /// This shouldn't be at 99999 or higher to prevent the singulo glitching out
    /// Will throw anything at the supplied mass or less that collides with the field.
    /// </summary>
    [DataField("maxMass")]
    public float MaxMass = 10000f;

    /// <summary>
    /// Should field vaporize garbage that collides with it?
    /// </summary>
    [DataField]
    public bool DestroyGarbage = true;
}
