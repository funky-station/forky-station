// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 Verm
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 ike709
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2022 Fishfish458
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 pointer-to-null
// SPDX-FileCopyrightText: 2021 Wrexbe
// SPDX-FileCopyrightText: 2021 Kara D
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Server.Storage.EntitySystems;
using Content.Server.Stunnable;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Atmos.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Content.Shared.PneumaticCannon;
using Content.Shared.Tools.Systems;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;

namespace Content.Server.PneumaticCannon;

public sealed class PneumaticCannonSystem : SharedPneumaticCannonSystem
{
    [Dependency] private readonly AtmosphereSystem _atmos = default!;
    [Dependency] private readonly GasTankSystem _gasTank = default!;
    [Dependency] private readonly GunSystem _gun = default!;
    [Dependency] private readonly StunSystem _stun = default!;
    [Dependency] private readonly ItemSlotsSystem _slots = default!;
    [Dependency] private readonly SharedToolSystem _toolSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PneumaticCannonComponent, InteractUsingEvent>(OnInteractUsing, before: new []{ typeof(StorageSystem) });
        SubscribeLocalEvent<PneumaticCannonComponent, GunShotEvent>(OnShoot);
        SubscribeLocalEvent<PneumaticCannonComponent, ContainerIsInsertingAttemptEvent>(OnContainerInserting);
        SubscribeLocalEvent<PneumaticCannonComponent, GunRefreshModifiersEvent>(OnGunRefreshModifiers);
    }

    private void OnInteractUsing(EntityUid uid, PneumaticCannonComponent component, InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!_toolSystem.HasQuality(args.Used, component.ToolModifyPower))
            return;

        var val = (int) component.Power;
        val = (val + 1) % (int) PneumaticCannonPower.Len;
        component.Power = (PneumaticCannonPower) val;

        Popup.PopupEntity(Loc.GetString("pneumatic-cannon-component-change-power",
            ("power", component.Power.ToString())), uid, args.User);

        component.ProjectileSpeed = GetProjectileSpeedFromPower(component);
        if (TryComp<GunComponent>(uid, out var gun))
            _gun.RefreshModifiers((uid, gun));

        args.Handled = true;
    }

    private void OnContainerInserting(EntityUid uid, PneumaticCannonComponent component, ContainerIsInsertingAttemptEvent args)
    {
        if (args.Container.ID != PneumaticCannonComponent.TankSlotId)
            return;

        if (!TryComp<GasTankComponent>(args.EntityUid, out var gas))
            return;

        // only accept tanks if it uses gas
        if (gas.Air.TotalMoles >= component.GasUsage && component.GasUsage > 0f)
            return;

        args.Cancel();
    }

    private void OnShoot(Entity<PneumaticCannonComponent> cannon, ref GunShotEvent args)
    {
        var (uid, component) = cannon;
        // require a gas tank if it uses gas
        var gas = GetGas(cannon);
        if (gas == null && component.GasUsage > 0f)
            return;

        if (component.Power == PneumaticCannonPower.High
            && _stun.TryUpdateParalyzeDuration(args.User, TimeSpan.FromSeconds(component.HighPowerStunTime)))
        {
            Popup.PopupEntity(Loc.GetString("pneumatic-cannon-component-power-stun",
                ("cannon", uid)), cannon, args.User);
        }

        // ignore gas stuff if the cannon doesn't use any
        if (gas == null)
            return;

        // this should always be possible, as we'll eject the gas tank when it no longer is
        var environment = _atmos.GetContainingMixture(cannon.Owner, false, true);
        var removed = _gasTank.RemoveAir(gas.Value, component.GasUsage);
        if (environment != null && removed != null)
        {
            _atmos.Merge(environment, removed);
        }

        if (gas.Value.Comp.Air.TotalMoles >= component.GasUsage)
            return;

        // eject gas tank
        _slots.TryEject(uid, PneumaticCannonComponent.TankSlotId, args.User, out _);
    }

    private void OnGunRefreshModifiers(Entity<PneumaticCannonComponent> ent, ref GunRefreshModifiersEvent args)
    {
        if (ent.Comp.ProjectileSpeed is { } speed)
            args.ProjectileSpeed = speed;
    }

    /// <summary>
    ///     Returns whether the pneumatic cannon has enough gas to shoot an item, as well as the tank itself.
    /// </summary>
    private Entity<GasTankComponent>? GetGas(EntityUid uid)
    {
        if (!Container.TryGetContainer(uid, PneumaticCannonComponent.TankSlotId, out var container) ||
            container is not ContainerSlot slot || slot.ContainedEntity is not {} contained)
            return null;

        return TryComp<GasTankComponent>(contained, out var gasTank) ? (contained, gasTank) : null;
    }

    private float GetProjectileSpeedFromPower(PneumaticCannonComponent component)
    {
        return component.Power switch
        {
            PneumaticCannonPower.High => component.BaseProjectileSpeed * 4f,
            PneumaticCannonPower.Medium => component.BaseProjectileSpeed,
            PneumaticCannonPower.Low or _ => component.BaseProjectileSpeed * 0.5f,
        };
    }
}
