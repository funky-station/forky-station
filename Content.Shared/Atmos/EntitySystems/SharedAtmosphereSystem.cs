// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 thetuerk <46725294+ThanosDeGraf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ArtisticRoomba <145879011+ArtisticRoomba@users.noreply.github.com>
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
