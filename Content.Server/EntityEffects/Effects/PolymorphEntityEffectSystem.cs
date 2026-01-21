// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Polymorph.Components;
using Content.Server.Polymorph.Systems;
using Content.Shared.EntityEffects;

namespace Content.Server.EntityEffects.Effects;

/// <summary>
/// Polymorphs this entity into another entity.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class PolymorphEntityEffectSystem : EntityEffectSystem<PolymorphableComponent, Shared.EntityEffects.Effects.Polymorph>
{
    [Dependency] private readonly PolymorphSystem _polymorph = default!;

    protected override void Effect(Entity<PolymorphableComponent> entity, ref EntityEffectEvent<Shared.EntityEffects.Effects.Polymorph> args)
    {
        _polymorph.PolymorphEntity(entity, args.Effect.Prototype);
    }
}
