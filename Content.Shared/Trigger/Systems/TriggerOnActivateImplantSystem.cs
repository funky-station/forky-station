// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2025 slarticodefast
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
