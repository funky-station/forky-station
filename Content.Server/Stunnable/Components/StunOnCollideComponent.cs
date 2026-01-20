// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-License-Identifier: MIT

using Content.Server.Stunnable.Systems;

namespace Content.Server.Stunnable.Components;

/// <summary>
/// Adds stun when it collides with an entity
/// </summary>
[RegisterComponent, Access(typeof(StunOnCollideSystem))]
public sealed partial class StunOnCollideComponent : Component
{
    // TODO: Can probably predict this.

    /// <summary>
    /// How long we are stunned for
    /// </summary>
    [DataField]
    public TimeSpan StunAmount;

    /// <summary>
    /// How long we are knocked down for
    /// </summary>
    [DataField]
    public TimeSpan KnockdownAmount;

    /// <summary>
    /// How long we are slowed down for
    /// </summary>
    [DataField]
    public TimeSpan SlowdownAmount;

    /// <summary>
    /// Multiplier for a mob's walking speed
    /// </summary>
    [DataField]
    public float WalkSpeedModifier = 1f;

    /// <summary>
    /// Multiplier for a mob's sprinting speed
    /// </summary>
    [DataField]
    public float SprintSpeedModifier = 1f;

    /// <summary>
    /// Refresh Stun or Slowdown on hit
    /// </summary>
    [DataField]
    public bool Refresh = true;

    /// <summary>
    /// Should the entity try and stand automatically after being knocked down?
    /// </summary>
    [DataField]
    public bool AutoStand = true;

    /// <summary>
    /// Should the entity drop their items upon first being knocked down?
    /// </summary>
    [DataField]
    public bool Drop = true;

    /// <summary>
    /// Fixture we track for the collision.
    /// </summary>
    [DataField("fixture")] public string FixtureID = "projectile";
}

