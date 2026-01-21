// SPDX-FileCopyrightText: 2025 āda <ss.adasts@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Stunnable;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

public sealed class KnockdownOnTriggerSystem : XOnTriggerSystem<KnockdownOnTriggerComponent>
{
    [Dependency] private readonly SharedStunSystem _stun = default!;

    protected override void OnTrigger(Entity<KnockdownOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        args.Handled |= _stun.TryKnockdown(
                        target,
                        ent.Comp.KnockdownAmount,
                        ent.Comp.Refresh,
                        ent.Comp.AutoStand,
                        ent.Comp.Drop,
                        true
                        );
    }
}
