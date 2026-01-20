// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2023 Ilya246
// SPDX-FileCopyrightText: 2023 faint
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos;
using JetBrains.Annotations;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems
{
    [UsedImplicitly]
    public sealed class GasPassiveVentSystem : EntitySystem
    {
        [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
        [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<GasPassiveVentComponent, AtmosDeviceUpdateEvent>(OnPassiveVentUpdated);
        }

        private void OnPassiveVentUpdated(EntityUid uid, GasPassiveVentComponent vent, ref AtmosDeviceUpdateEvent args)
        {
            var environment = _atmosphereSystem.GetContainingMixture(uid, args.Grid, args.Map, true, true);

            if (environment == null)
                return;

            if (!_nodeContainer.TryGetNode(uid, vent.InletName, out PipeNode? inlet))
                return;

            var inletAir = inlet.Air.RemoveRatio(1f);
            var envAir = environment.RemoveRatio(1f);

            var mergeAir = new GasMixture(inletAir.Volume + envAir.Volume);
            _atmosphereSystem.Merge(mergeAir, inletAir);
            _atmosphereSystem.Merge(mergeAir, envAir);

            _atmosphereSystem.Merge(inlet.Air, mergeAir.RemoveVolume(inletAir.Volume));
            _atmosphereSystem.Merge(environment, mergeAir);
        }
    }
}
