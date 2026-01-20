// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Content.Server.Light.Components;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Power;

namespace Content.Server.Light.EntitySystems
{
    public sealed class LitOnPoweredSystem : EntitySystem
    {
        [Dependency] private readonly SharedPointLightSystem _lights = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<LitOnPoweredComponent, PowerChangedEvent>(OnPowerChanged);
            SubscribeLocalEvent<LitOnPoweredComponent, PowerNetBatterySupplyEvent>(OnPowerSupply);
        }

        private void OnPowerChanged(EntityUid uid, LitOnPoweredComponent component, ref PowerChangedEvent args)
        {
            if (_lights.TryGetLight(uid, out var light))
            {
                _lights.SetEnabled(uid, args.Powered, light);
            }
        }

        private void OnPowerSupply(EntityUid uid, LitOnPoweredComponent component, ref PowerNetBatterySupplyEvent args)
        {
            if (_lights.TryGetLight(uid, out var light))
            {
                _lights.SetEnabled(uid, args.Supply, light);
            }
        }
    }
}
