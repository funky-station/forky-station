// SPDX-FileCopyrightText: 2023 themias <89101928+themias@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TaralGit <76408146+TaralGit@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Fildrance <fildrance@gmail.com>
// SPDX-FileCopyrightText: 2026 mq <113324899+mqole@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Containers;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;

namespace Content.Shared.Weapons.Ranged.Systems;

public partial class SharedGunSystem
{
    private void InitializeClothing()
    {
        SubscribeLocalEvent<ClothingSlotAmmoProviderComponent, TakeAmmoEvent>(OnClothingTakeAmmo);
        SubscribeLocalEvent<ClothingSlotAmmoProviderComponent, GetAmmoCountEvent>(OnClothingAmmoCount);
    }

    private void OnClothingTakeAmmo(EntityUid uid, ClothingSlotAmmoProviderComponent component, TakeAmmoEvent args)
    {
        var getConnectedContainerEvent = new GetConnectedContainerEvent();
        RaiseLocalEvent(uid, ref getConnectedContainerEvent);
        if (!getConnectedContainerEvent.ContainerEntity.HasValue)
            return;

        RaiseLocalEvent(getConnectedContainerEvent.ContainerEntity.Value, args);
    }

    private void OnClothingAmmoCount(EntityUid uid, ClothingSlotAmmoProviderComponent component, ref GetAmmoCountEvent args)
    {
        var getConnectedContainerEvent = new GetConnectedContainerEvent();
        RaiseLocalEvent(uid, ref getConnectedContainerEvent);
        if (!getConnectedContainerEvent.ContainerEntity.HasValue)
            return;

        RaiseLocalEvent(getConnectedContainerEvent.ContainerEntity.Value, ref args);
    }
}
