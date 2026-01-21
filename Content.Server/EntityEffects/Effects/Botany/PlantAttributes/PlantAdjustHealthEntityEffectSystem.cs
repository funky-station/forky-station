// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-License-Identifier: MIT

using Content.Server.Botany.Components;
using Content.Server.Botany.Systems;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects.Botany.PlantAttributes;

namespace Content.Server.EntityEffects.Effects.Botany.PlantAttributes;

public sealed partial class PlantAdjustHealthEntityEffectSystem : EntityEffectSystem<PlantHolderComponent, PlantAdjustHealth>
{
    [Dependency] private readonly PlantHolderSystem _plantHolder = default!;

    protected override void Effect(Entity<PlantHolderComponent> entity, ref EntityEffectEvent<PlantAdjustHealth> args)
    {
        if (entity.Comp.Seed == null || entity.Comp.Dead)
            return;

        entity.Comp.Health += args.Effect.Amount;
        _plantHolder.CheckHealth(entity, entity.Comp);
    }
}
