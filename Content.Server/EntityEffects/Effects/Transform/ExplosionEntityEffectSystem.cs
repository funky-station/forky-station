// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-License-Identifier: MIT

using Content.Server.Explosion.EntitySystems;
using Content.Shared.EntityEffects;
using ExplosionEffect = Content.Shared.EntityEffects.Effects.Transform.Explosion;

namespace Content.Server.EntityEffects.Effects.Transform;

/// <summary>
/// Creates an explosion at this entity's position.
/// Intensity is modified by scale.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class ExplosionEntityEffectSystem : EntityEffectSystem<TransformComponent, ExplosionEffect>
{
    [Dependency] private readonly ExplosionSystem _explosion = default!;

    protected override void Effect(Entity<TransformComponent> entity, ref EntityEffectEvent<ExplosionEffect> args)
    {
        var intensity = MathF.Min(args.Effect.IntensityPerUnit * args.Scale, args.Effect.MaxTotalIntensity);

        _explosion.QueueExplosion(
            entity,
            args.Effect.ExplosionType,
            intensity,
            args.Effect.IntensitySlope,
            args.Effect.MaxIntensity,
            args.Effect.TileBreakScale);
    }
}
