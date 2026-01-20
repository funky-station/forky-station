// SPDX-FileCopyrightText: 2025 ArtisticRoomba
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Kevin Zheng
// SPDX-FileCopyrightText: 2022 20kdc
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Atmos;
using Content.Shared.Guidebook;

namespace Content.Server.Atmos.Piping.Trinary.Components
{
    [RegisterComponent]
    public sealed partial class PressureControlledValveComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("inlet")]
        public string InletName { get; set; } = "inlet";

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("control")]
        public string ControlName { get; set; } = "control";

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("outlet")]
        public string OutletName { get; set; } = "outlet";

        [ViewVariables(VVAccess.ReadOnly)]
        [DataField("enabled")]
        public bool Enabled { get; set; } = false;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("gain")]
        public float Gain { get; set; } = 10;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("threshold")]
        [GuidebookData]
        public float Threshold { get; set; } = Atmospherics.OneAtmosphere;

        [DataField("maxTransferRate")]
        public float MaxTransferRate { get; set; } = Atmospherics.MaxTransferRate;
    }
}
