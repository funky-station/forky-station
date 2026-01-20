// SPDX-FileCopyrightText: 2025 Nemanja
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 Jezithyr
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2023 Kevin Zheng
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 ColdAutumnRain
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using Robust.Shared.Prototypes;

namespace Content.Server.Atmos.Reactions
{
    [Prototype]
    public sealed partial class GasReactionPrototype : IPrototype
    {
        [ViewVariables]
        [IdDataField]
        public string ID { get; private set; } = default!;

        /// <summary>
        ///     Minimum gas amount requirements.
        /// </summary>
        [DataField("minimumRequirements", customTypeSerializer: typeof(GasArraySerializer))]
        public float[] MinimumRequirements { get; private set; } = new float[Atmospherics.TotalNumberOfGases];

        /// <summary>
        ///     Maximum temperature requirement.
        /// </summary>
        [DataField("maximumTemperature")]
        public float MaximumTemperatureRequirement { get; private set; } = float.MaxValue;

        /// <summary>
        ///     Minimum temperature requirement.
        /// </summary>
        [DataField("minimumTemperature")]
        public float MinimumTemperatureRequirement { get; private set; } = Atmospherics.TCMB;

        /// <summary>
        ///     Minimum energy requirement.
        /// </summary>
        [DataField("minimumEnergy")]
        public float MinimumEnergyRequirement { get; private set; } = 0f;

        /// <summary>
        ///     Lower numbers are checked/react later than higher numbers.
        ///     If two reactions have the same priority, they may happen in either order.
        /// </summary>
        [DataField("priority")]
        public int Priority { get; private set; } = int.MinValue;

        /// <summary>
        ///     A list of effects this will produce.
        /// </summary>
        [DataField("effects")] private List<IGasReactionEffect> _effects = new();

        /// <summary>
        /// Process all reaction effects.
        /// </summary>
        /// <param name="mixture">The gas mixture to react</param>
        /// <param name="holder">The container of this gas mixture</param>
        /// <param name="atmosphereSystem">The atmosphere system</param>
        /// <param name="heatScale">Scaling factor that should be applied to all heat input or outputs.</param>
        public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
        {
            var result = ReactionResult.NoReaction;

            foreach (var effect in _effects)
            {
                result |= effect.React(mixture, holder, atmosphereSystem, heatScale);
            }

            return result;
        }
    }
}
