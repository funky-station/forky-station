// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 778b <33431126+778b@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 mq <113324899+mqole@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Weapons.Ranged;

namespace Content.Client.Weapons.Ranged.Systems;

public sealed partial class GunSystem
{
    protected override void InitializeMagazine()
    {
        base.InitializeMagazine();
        SubscribeLocalEvent<MagazineAmmoProviderComponent, UpdateAmmoCounterEvent>(OnMagazineAmmoUpdate);
        SubscribeLocalEvent<MagazineAmmoProviderComponent, AmmoCounterControlEvent>(OnMagazineControl);
    }

    private void OnMagazineAmmoUpdate(Entity<MagazineAmmoProviderComponent> ent, ref UpdateAmmoCounterEvent args)
    {
        var magEnt = GetMagazineEntity(ent);

        if (magEnt == null)
        {
            if (args.Control is DefaultStatusControl control)
            {
                control.Update(0, 0);
            }

            return;
        }

        RaiseLocalEvent(magEnt.Value, args, false);
    }

    private void OnMagazineControl(Entity<MagazineAmmoProviderComponent> ent, ref AmmoCounterControlEvent args)
    {
        var magEnt = GetMagazineEntity(ent);
        if (magEnt == null)
            return;
        RaiseLocalEvent(magEnt.Value, args, false);
    }
}
