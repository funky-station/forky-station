// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Slippery;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

public sealed partial class TriggerOnSlipSystem : TriggerOnXSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnSlipComponent, SlipEvent>(OnSlip);
    }

    private void OnSlip(Entity<TriggerOnSlipComponent> ent, ref SlipEvent args)
    {
        Trigger.Trigger(ent.Owner, args.Slipped, ent.Comp.KeyOut);
    }
}
