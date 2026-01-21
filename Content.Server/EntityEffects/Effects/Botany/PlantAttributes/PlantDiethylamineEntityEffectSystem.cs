// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-License-Identifier: MIT

using Content.Server.Botany.Components;
using Content.Server.Botany.Systems;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects.Botany.PlantAttributes;
using Robust.Shared.Random;

namespace Content.Server.EntityEffects.Effects.Botany.PlantAttributes;

public sealed partial class PlantDiethylamineEntityEffectSystem : EntityEffectSystem<PlantHolderComponent, PlantDiethylamine>
{
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override void Effect(Entity<PlantHolderComponent> entity, ref EntityEffectEvent<PlantDiethylamine> args)
    {
        if (entity.Comp.Seed == null || entity.Comp.Dead || entity.Comp.Seed.Immutable)
            return;

        if (_random.Prob(0.1f))
        {
            entity.Comp.Seed!.Lifespan++;
        }

        if (_random.Prob(0.1f))
        {
            entity.Comp.Seed!.Endurance++;
        }
    }
}
