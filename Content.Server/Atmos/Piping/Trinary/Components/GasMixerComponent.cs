// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 20kdc
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 moonheart08
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.Piping.Trinary.EntitySystems;
using Content.Shared.Atmos;

namespace Content.Server.Atmos.Piping.Trinary.Components
{
    [RegisterComponent]
    [Access(typeof(GasMixerSystem))]
    public sealed partial class GasMixerComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("enabled")]
        public bool Enabled = true;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("inletOne")]
        public string InletOneName = "inletOne";

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("inletTwo")]
        public string InletTwoName = "inletTwo";

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("outlet")]
        public string OutletName = "outlet";

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("targetPressure")]
        public float TargetPressure = Atmospherics.OneAtmosphere;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("maxTargetPressure")]
        public float MaxTargetPressure = Atmospherics.MaxOutputPressure;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("inletOneConcentration")]
        public float InletOneConcentration = 0.5f;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("inletTwoConcentration")]
        public float InletTwoConcentration = 0.5f;
    }
}
