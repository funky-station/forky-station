// SPDX-FileCopyrightText: 2021-2022, 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021, 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2023 KP <13428215+nok-ko@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson <karakkaraz@gmail.com>
// SPDX-FileCopyrightText: 2025 IProduceWidgets <107586145+IProduceWidgets@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Damage.Components;
using Content.Shared.Damage;
using Content.Shared.Throwing;

namespace Content.Server.Damage.Systems
{
    /// <summary>
    /// Damages the thrown item when it lands.
    /// </summary>
    public sealed class DamageOnLandSystem : EntitySystem
    {
        [Dependency] private readonly Shared.Damage.Systems.DamageableSystem _damageableSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<DamageOnLandComponent, LandEvent>(DamageOnLand);
        }

        private void DamageOnLand(EntityUid uid, DamageOnLandComponent component, ref LandEvent args)
        {
            _damageableSystem.TryChangeDamage(uid, component.Damage, component.IgnoreResistances);
        }
    }
}
