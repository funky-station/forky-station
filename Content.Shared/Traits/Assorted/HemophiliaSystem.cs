// SPDX-FileCopyrightText: 2025 ScarKy0
// SPDX-FileCopyrightText: 2025 TheFlyingSentry
// SPDX-License-Identifier: MIT

using Content.Shared.Body.Events;
using Content.Shared.StatusEffectNew;

namespace Content.Shared.Traits.Assorted;

public sealed class HemophiliaSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<HemophiliaStatusEffectComponent, StatusEffectRelayedEvent<BleedModifierEvent>>(OnBleedModifier);
    }

    private void OnBleedModifier(Entity<HemophiliaStatusEffectComponent> ent, ref StatusEffectRelayedEvent<BleedModifierEvent> args)
    {
        var ev = args.Args;
        ev.BleedReductionAmount *= ent.Comp.BleedReductionMultiplier;
        ev.BleedAmount *= ent.Comp.BleedAmountMultiplier;
        args.Args = ev;
    }
}
