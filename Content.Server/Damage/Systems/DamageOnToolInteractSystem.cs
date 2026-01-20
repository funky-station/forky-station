// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 Verm
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 Darkie
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Mith-randalf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Logs;
using Content.Server.Damage.Components;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using ItemToggleComponent = Content.Shared.Item.ItemToggle.Components.ItemToggleComponent;

namespace Content.Server.Damage.Systems
{
    public sealed class DamageOnToolInteractSystem : EntitySystem
    {
        [Dependency] private readonly Shared.Damage.Systems.DamageableSystem _damageableSystem = default!;
        [Dependency] private readonly IAdminLogManager _adminLogger = default!;
        [Dependency] private readonly SharedToolSystem _toolSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<DamageOnToolInteractComponent, InteractUsingEvent>(OnInteracted);
        }

        private void OnInteracted(EntityUid uid, DamageOnToolInteractComponent component, InteractUsingEvent args)
        {
            if (args.Handled)
                return;

            if (!TryComp<ItemToggleComponent>(args.Used, out var itemToggle))
                return;

            if (component.WeldingDamage is {} weldingDamage
            && TryComp(args.Used, out WelderComponent? welder)
            && itemToggle.Activated
            && !welder.TankSafe)
            {
                if (_damageableSystem.TryChangeDamage(args.Target, weldingDamage, out var dmg, origin: args.User))
                {
                    _adminLogger.Add(LogType.Damaged,
                        $"{ToPrettyString(args.User):user} used {ToPrettyString(args.Used):used} as a welder to deal {dmg.GetTotal():damage} damage to {ToPrettyString(args.Target):target}");
                }

                args.Handled = true;
            }
            else if (component.DefaultDamage is {} damage
                && _toolSystem.HasQuality(args.Used, component.Tools))
            {
                if (_damageableSystem.TryChangeDamage(args.Target, damage, out var dmg, origin: args.User))
                {
                    _adminLogger.Add(LogType.Damaged,
                        $"{ToPrettyString(args.User):user} used {ToPrettyString(args.Used):used} as a tool to deal {dmg.GetTotal():damage} damage to {ToPrettyString(args.Target):target}");
                }

                args.Handled = true;
            }
        }
    }
}
