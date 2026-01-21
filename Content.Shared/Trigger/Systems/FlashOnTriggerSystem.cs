// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Flash;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

public sealed class FlashOnTriggerSystem : XOnTriggerSystem<FlashOnTriggerComponent>
{
    [Dependency] private readonly SharedFlashSystem _flash = default!;

    protected override void OnTrigger(Entity<FlashOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        _flash.FlashArea(target, args.User, ent.Comp.Range, ent.Comp.Duration, probability: ent.Comp.Probability);
        args.Handled = true;
    }
}
