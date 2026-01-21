// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects.Body;

namespace Content.Server.EntityEffects.Effects.Body;

/// <summary>
/// This effect adjusts a respirator's saturation value.
/// The saturation adjustment is modified by scale.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class OxygenateEntityEffectsSystem : EntityEffectSystem<RespiratorComponent, Oxygenate>
{
    [Dependency] private readonly RespiratorSystem _respirator = default!;
    protected override void Effect(Entity<RespiratorComponent> entity, ref EntityEffectEvent<Oxygenate> args)
    {
        _respirator.UpdateSaturation(entity, args.Scale * args.Effect.Factor, entity.Comp);
    }
}
