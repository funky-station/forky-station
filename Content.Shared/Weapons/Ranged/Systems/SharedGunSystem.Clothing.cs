// SPDX-FileCopyrightText: 2026 mq
// SPDX-FileCopyrightText: 2025 Fildrance
// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2023 themias
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 TaralGit
// SPDX-FileCopyrightText: 2023 Nemanja
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
