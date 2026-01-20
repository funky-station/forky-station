// SPDX-FileCopyrightText: 2025 SlamBamActionman
// SPDX-FileCopyrightText: 2024 MilenVolf
// SPDX-FileCopyrightText: 2023 Arendian
// SPDX-FileCopyrightText: 2024 Arendian
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Projectiles;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ProjectileComponent : Component
{
    /// <summary>
    ///     The angle of the fired projectile.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Angle Angle;

    /// <summary>
    ///     The effect that appears when a projectile collides with an entity.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId? ImpactEffect;

    /// <summary>
    ///     User that shot this projectile.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Shooter;

    /// <summary>
    ///     Weapon used to shoot.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Weapon;

    /// <summary>
    ///     The projectile spawns inside the shooter most of the time, this prevents entities from shooting themselves.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IgnoreShooter = true;

    /// <summary>
    ///     The amount of damage the projectile will do.
    /// </summary>
    [DataField(required: true)] [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier Damage = new();

    /// <summary>
    ///     If the projectile should be deleted on collision.
    /// </summary>
    [DataField]
    public bool DeleteOnCollide = true;

    /// <summary>
    ///     Ignore all damage resistances the target has.
    /// </summary>
    [DataField]
    public bool IgnoreResistances = false;

    /// <summary>
    ///     Get that juicy FPS hit sound.
    /// </summary>
    [DataField]
    public SoundSpecifier? SoundHit;

    /// <summary>
    ///     Force the projectiles sound to play rather than potentially playing the entity's sound.
    /// </summary>
    [DataField]
    public bool ForceSound = false;

    /// <summary>
    ///     Whether this projectile will only collide with entities if it was shot from a gun (if <see cref="Weapon"/> is not null).
    /// </summary>
    [DataField]
    public bool OnlyCollideWhenShot = false;

    /// <summary>
    ///     If true, the projectile has hit enough targets and should no longer interact with further collisions pending deletion.
    /// </summary>
    [DataField]
    public bool ProjectileSpent;

    /// <summary>
    ///     When a projectile has this threshold set, it will continue to penetrate entities until the damage dealt reaches this threshold.
    /// </summary>
    [DataField]
    public FixedPoint2 PenetrationThreshold = FixedPoint2.Zero;

    /// <summary>
    ///     If set, the projectile will not penetrate objects that lack the ability to take these damage types.
    /// </summary>
    [DataField]
    public List<string>? PenetrationDamageTypeRequirement;

    /// <summary>
    ///     Tracks the amount of damage dealt for penetration purposes.
    /// </summary>
    [DataField]
    public FixedPoint2 PenetrationAmount = FixedPoint2.Zero;
}
