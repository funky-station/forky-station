// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Implants.Components;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

public sealed partial class TriggerOnActivateImplantSystem : TriggerOnXSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnActivateImplantComponent, ActivateImplantEvent>(OnActivateImplant);
    }

    private void OnActivateImplant(Entity<TriggerOnActivateImplantComponent> ent, ref ActivateImplantEvent args)
    {
        Trigger.Trigger(ent.Owner, args.Performer, ent.Comp.KeyOut);
        args.Handled = true;
    }
}
