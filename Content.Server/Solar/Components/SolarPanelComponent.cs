// SPDX-FileCopyrightText: 2025 ArtisticRoomba
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Peptide90
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2020 20kdc
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 py01
// SPDX-License-Identifier: MIT

using Content.Server.Solar.EntitySystems;
using Content.Shared.Guidebook;

namespace Content.Server.Solar.Components
{

    /// <summary>
    ///     This is a solar panel.
    ///     It generates power from the sun based on coverage.
    /// </summary>
    [RegisterComponent]
    [Access(typeof(PowerSolarSystem))]
    public sealed partial class SolarPanelComponent : Component
    {
        /// <summary>
        /// Maximum supply output by this panel (coverage = 1)
        /// </summary>
        [DataField("maxSupply")]
        [GuidebookData]
        public int MaxSupply = 750;

        /// <summary>
        /// Current coverage of this panel (from 0 to 1).
        /// This is updated by <see cref='PowerSolarSystem'/>.
        /// DO NOT WRITE WITHOUT CALLING UpdateSupply()!
        /// </summary>
        [ViewVariables]
        public float Coverage { get; set; } = 0;
    }
}
