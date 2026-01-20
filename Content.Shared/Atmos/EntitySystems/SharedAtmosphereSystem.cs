// SPDX-FileCopyrightText: 2026 ArtisticRoomba
// SPDX-FileCopyrightText: 2025 Nemanja
// SPDX-FileCopyrightText: 2025 thetuerk
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Shared.Atmos.Prototypes;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;

namespace Content.Shared.Atmos.EntitySystems
{
    public abstract partial class SharedAtmosphereSystem : EntitySystem
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly SharedInternalsSystem _internals = default!;
        [Dependency] private readonly IConfigurationManager _cfg = default!;

        private EntityQuery<InternalsComponent> _internalsQuery;

        public override void Initialize()
        {
            base.Initialize();

            _internalsQuery = GetEntityQuery<InternalsComponent>();

            InitializeBreathTool();
            InitializeGases();
            InitializeCVars();
        }

        public GasPrototype GetGas(int gasId) => GasPrototypes[gasId];

        public GasPrototype GetGas(Gas gasId) => GasPrototypes[(int) gasId];

        public IEnumerable<GasPrototype> Gases => GasPrototypes;
    }
}
