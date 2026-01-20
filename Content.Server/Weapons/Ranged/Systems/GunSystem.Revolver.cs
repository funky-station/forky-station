// SPDX-FileCopyrightText: 2026 mq
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Shared.Weapons.Ranged.Components;

namespace Content.Server.Weapons.Ranged.Systems;

public sealed partial class GunSystem
{
    protected override void SpinRevolver(Entity<RevolverAmmoProviderComponent> ent, EntityUid? user = null)
    {
        base.SpinRevolver(ent, user);
        var index = Random.Next(ent.Comp.Capacity);

        if (ent.Comp.CurrentIndex == index)
            return;

        ent.Comp.CurrentIndex = index;
        Dirty(ent);
    }
}
