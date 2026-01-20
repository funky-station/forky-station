// SPDX-FileCopyrightText: 2024 Jezithyr
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;

namespace Content.Server.Atmos
{
    [ImplicitDataDefinitionForInheritors]
    public partial interface IGasReactionEffect
    {
        /// <summary>
        /// Process this reaction effect.
        /// </summary>
        /// <param name="mixture">The gas mixture to react</param>
        /// <param name="holder">The container of this gas mixture</param>
        /// <param name="atmosphereSystem">The atmosphere system</param>
        /// <param name="heatScale">Scaling factor that should be applied to all heat input or outputs.</param>
        ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem,
            float heatScale);
    }
}
