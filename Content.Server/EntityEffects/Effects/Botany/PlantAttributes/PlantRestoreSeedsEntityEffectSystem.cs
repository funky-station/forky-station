// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-License-Identifier: MIT

using Content.Server.Botany.Components;
using Content.Server.Botany.Systems;
using Content.Server.Popups;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects.Botany.PlantAttributes;

namespace Content.Server.EntityEffects.Effects.Botany.PlantAttributes;

public sealed partial class PlantRestoreSeedsEntityEffectSystem : EntityEffectSystem<PlantHolderComponent, PlantRestoreSeeds>
{
    [Dependency] private readonly PopupSystem _popup = default!;

    protected override void Effect(Entity<PlantHolderComponent> entity, ref EntityEffectEvent<PlantRestoreSeeds> args)
    {
        if (entity.Comp.Seed == null || entity.Comp.Dead || entity.Comp.Seed.Immutable)
            return;

        if (!entity.Comp.Seed.Seedless)
            return;

        _popup.PopupEntity(Loc.GetString("botany-plant-seedsrestored"), entity);
        entity.Comp.Seed.Seedless = false;
    }
}
