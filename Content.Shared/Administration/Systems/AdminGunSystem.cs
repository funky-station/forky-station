// SPDX-FileCopyrightText: 2025 Pok
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Shared.Administration.Components;
using Content.Shared.Weapons.Ranged.Events;

namespace Content.Shared.Administration.Systems;

public sealed class AdminGunSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<AdminMinigunComponent, GunRefreshModifiersEvent>(OnGunRefreshModifiers);
    }

    private void OnGunRefreshModifiers(Entity<AdminMinigunComponent> ent, ref GunRefreshModifiersEvent args)
    {
        args.FireRate = 15;
    }
}
