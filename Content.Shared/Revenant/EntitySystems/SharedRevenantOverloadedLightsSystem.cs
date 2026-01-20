// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-License-Identifier: MIT

using Content.Shared.Revenant.Components;

namespace Content.Shared.Revenant.EntitySystems;

/// <summary>
/// This handles...
/// </summary>
public abstract class SharedRevenantOverloadedLightsSystem : EntitySystem
{
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var enumerator = EntityQueryEnumerator<RevenantOverloadedLightsComponent>();

        while (enumerator.MoveNext(out var uid, out var comp))
        {
            comp.Accumulator += frameTime;

            if (comp.Accumulator < comp.ZapDelay)
                continue;

            OnZap((uid, comp));
            RemCompDeferred(uid, comp);
        }
    }

    protected abstract void OnZap(Entity<RevenantOverloadedLightsComponent> component);
}
