// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Administration.Systems;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

public sealed class RejuvenateOnTriggerSystem : XOnTriggerSystem<RejuvenateOnTriggerComponent>
{
    [Dependency] private readonly RejuvenateSystem _rejuvenate = default!;

    protected override void OnTrigger(Entity<RejuvenateOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        _rejuvenate.PerformRejuvenate(target);
        args.Handled = true;
    }
}
