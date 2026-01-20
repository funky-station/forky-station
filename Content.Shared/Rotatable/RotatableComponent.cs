// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Rotatable;

/// <summary>
/// Allows an entity to be rotated by using a verb.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RotatableComponent : Component
{
    /// <summary>
    /// If true, this entity can be rotated even while anchored.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool RotateWhileAnchored;

    /// <summary>
    /// If true, will rotate entity in players direction when pulled
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool RotateWhilePulling = true;

    /// <summary>
    /// The angular value to change when using the rotate verbs.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Angle Increment = Angle.FromDegrees(90);
}
