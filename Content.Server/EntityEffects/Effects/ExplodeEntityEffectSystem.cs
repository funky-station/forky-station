// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Explosion.EntitySystems;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects;
using Content.Shared.Explosion.Components;

namespace Content.Server.EntityEffects.Effects;

/// <summary>
/// Makes this entity explode using its <see cref="ExplosiveComponent"/>.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class ExplodeEntityEffectSystem : EntityEffectSystem<ExplosiveComponent, Explode>
{
    [Dependency] private readonly ExplosionSystem _explosion = default!;

    protected override void Effect(Entity<ExplosiveComponent> entity, ref EntityEffectEvent<Explode> args)
    {
        _explosion.TriggerExplosive(entity, entity, args.Effect.Delete, args.Effect.Intensity, args.Effect.Radius, args.User);
    }
}
