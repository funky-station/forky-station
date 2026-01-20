// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Morb
// SPDX-FileCopyrightText: 2022 Kevin Zheng
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Atmos.Prototypes
{
    [Prototype]
    public sealed partial class GasPrototype : IPrototype
    {
        [DataField("name")] public string Name { get; set; } = "";

        // TODO: Control gas amount necessary for overlay to appear
        // TODO: Add interfaces for gas behaviours e.g. breathing, burning

        [ViewVariables]
        [IdDataField]
        public string ID { get; private set; } = default!;

        /// <summary>
        ///     Specific heat for gas.
        /// </summary>
        [DataField("specificHeat")]
        public float SpecificHeat { get; private set; }

        /// <summary>
        /// Heat capacity ratio for gas
        /// </summary>
        [DataField("heatCapacityRatio")]
        public float HeatCapacityRatio { get; private set; } = 1.4f;

        /// <summary>
        /// Molar mass of gas
        /// </summary>
        [DataField("molarMass")]
        public float MolarMass { get; set; } = 1f;


        /// <summary>
        ///     Minimum amount of moles for this gas to be visible.
        /// </summary>
        [DataField("gasMolesVisible")]
        public float GasMolesVisible { get; private set; } = 0.25f;

        /// <summary>
        ///     Visibility for this gas will be max after this value.
        /// </summary>
        public float GasMolesVisibleMax => GasMolesVisible * GasVisibilityFactor;

        [DataField("gasVisbilityFactor")]
        public float GasVisibilityFactor = Atmospherics.FactorGasVisibleMax;

        /// <summary>
        ///     If this reagent is in gas form, this is the path to the overlay that will be used to make the gas visible.
        /// </summary>
        [DataField("gasOverlayTexture")]
        public string GasOverlayTexture { get; private set; } = string.Empty;

        /// <summary>
        ///     If this reagent is in gas form, this will be the path to the RSI sprite that will be used to make the gas visible.
        /// </summary>
        [DataField("gasOverlayState")]
        public string GasOverlayState { get; set; } = string.Empty;

        /// <summary>
        ///     State for the gas RSI overlay.
        /// </summary>
        [DataField("gasOverlaySprite")]
        public string GasOverlaySprite { get; set; } = string.Empty;

        /// <summary>
        /// Path to the tile overlay used when this gas appears visible.
        /// </summary>
        [DataField("overlayPath")]
        public string OverlayPath { get; private set; } = string.Empty;

        /// <summary>
        /// The reagent that this gas will turn into when inhaled.
        /// </summary>
        [DataField("reagent", customTypeSerializer:typeof(PrototypeIdSerializer<ReagentPrototype>))]
        public string? Reagent { get; private set; } = default!;

        [DataField("color")] public string Color { get; private set; } = string.Empty;

        [DataField("pricePerMole")]
        public float PricePerMole { get; set; } = 0;
    }
}
