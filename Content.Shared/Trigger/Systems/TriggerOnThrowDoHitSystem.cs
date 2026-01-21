// SPDX-FileCopyrightText: 2025 āda <ss.adasts@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Throwing;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

public sealed partial class TriggerOnThrowDoHitSystem : TriggerOnXSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnThrowDoHitComponent, ThrowDoHitEvent>(OnHit);
    }

    private void OnHit(Entity<TriggerOnThrowDoHitComponent> ent, ref ThrowDoHitEvent args)
    {
        Trigger.Trigger(ent.Owner, args.Target, ent.Comp.KeyOut);
    }
}
