// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Trigger.Components.Triggers;
using Content.Shared.Weapons.Ranged.Events;

namespace Content.Shared.Trigger.Systems;
public sealed partial class TriggerOnEmptyGunshotSystem : TriggerOnXSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnEmptyGunshotComponent, OnEmptyGunShotEvent>(OnEmptyGunShot);
    }

    private void OnEmptyGunShot(Entity<TriggerOnEmptyGunshotComponent> ent, ref OnEmptyGunShotEvent args)
    {
        Trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
    }
}
