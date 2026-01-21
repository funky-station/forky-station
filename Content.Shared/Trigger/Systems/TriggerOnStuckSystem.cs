// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Sticky;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

public sealed class TriggerOnStuckSystem : TriggerOnXSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnStuckComponent, EntityStuckEvent>(OnStuck);
    }

    private void OnStuck(Entity<TriggerOnStuckComponent> ent, ref EntityStuckEvent args)
    {
        Trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
    }
}
