// SPDX-FileCopyrightText: 2026 mq
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-License-Identifier: MIT

using Content.Shared.Weapons.Ranged.Components;

namespace Content.Shared.Weapons.Ranged.Systems;

public partial class SharedGunSystem
{
    public void SetEnabled(Entity<AutoShootGunComponent> ent, bool status)
    {
        ent.Comp.Enabled = status;
    }
}
