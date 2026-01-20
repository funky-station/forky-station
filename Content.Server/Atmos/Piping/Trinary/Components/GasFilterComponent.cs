// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 20kdc
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Atmos;

namespace Content.Server.Atmos.Piping.Trinary.Components
{
    [RegisterComponent]
    public sealed partial class GasFilterComponent : Component
    {
        [DataField]
        public bool Enabled = true;

        [DataField("inlet")]
        public string InletName = "inlet";

        [DataField("filter")]
        public string FilterName = "filter";

        [DataField("outlet")]
        public string OutletName = "outlet";

        [DataField]
        public float TransferRate = Atmospherics.MaxTransferRate;

        [DataField]
        public float MaxTransferRate = Atmospherics.MaxTransferRate;

        [DataField]
        public Gas? FilteredGas;
    }
}
