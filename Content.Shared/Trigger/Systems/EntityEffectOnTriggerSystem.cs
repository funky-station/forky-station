// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.EntityEffects;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

public sealed class EntityEffectOnTriggerSystem : XOnTriggerSystem<EntityEffectOnTriggerComponent>
{
    [Dependency] private readonly SharedEntityEffectsSystem _effects = default!;

    protected override void OnTrigger(Entity<EntityEffectOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        _effects.ApplyEffects(target, ent.Comp.Effects, ent.Comp.Scale);
        args.Handled = true;
    }
}
