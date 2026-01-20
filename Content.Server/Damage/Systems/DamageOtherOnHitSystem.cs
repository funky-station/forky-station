// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 SlamBamActionman
// SPDX-FileCopyrightText: 2024 Mervill
// SPDX-FileCopyrightText: 2024 Cojoke
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2024 LordCarve
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2023 Jezithyr
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Logs;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Camera;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Database;
using Content.Shared.Effects;
using Content.Shared.Mobs.Components;
using Content.Shared.Throwing;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;

namespace Content.Server.Damage.Systems;

public sealed class DamageOtherOnHitSystem : SharedDamageOtherOnHitSystem
{
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly GunSystem _guns = default!;
    [Dependency] private readonly Shared.Damage.Systems.DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedCameraRecoilSystem _sharedCameraRecoil = default!;
    [Dependency] private readonly SharedColorFlashEffectSystem _color = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DamageOtherOnHitComponent, ThrowDoHitEvent>(OnDoHit);
    }

    private void OnDoHit(EntityUid uid, DamageOtherOnHitComponent component, ThrowDoHitEvent args)
    {
        if (TerminatingOrDeleted(args.Target))
            return;

        var dmg = _damageable.ChangeDamage(args.Target, component.Damage * _damageable.UniversalThrownDamageModifier, component.IgnoreResistances, origin: args.Component.Thrower);

        // Log damage only for mobs. Useful for when people throw spears at each other, but also avoids log-spam when explosions send glass shards flying.
        if (HasComp<MobStateComponent>(args.Target))
            _adminLogger.Add(LogType.ThrowHit, $"{ToPrettyString(args.Target):target} received {dmg.GetTotal():damage} damage from collision");

        if (!dmg.Empty)
        {
            _color.RaiseEffect(Color.Red, [args.Target], Filter.Pvs(args.Target, entityManager: EntityManager));
        }

        _guns.PlayImpactSound(args.Target, dmg, null, false);
        if (TryComp<PhysicsComponent>(uid, out var body) && body.LinearVelocity.LengthSquared() > 0f)
        {
            var direction = body.LinearVelocity.Normalized();
            _sharedCameraRecoil.KickCamera(args.Target, direction);
        }
    }
}
