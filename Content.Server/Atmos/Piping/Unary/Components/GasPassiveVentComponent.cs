// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

namespace Content.Server.Atmos.Piping.Unary.Components
{
    [RegisterComponent]
    public sealed partial class GasPassiveVentComponent : Component
    {
        [DataField("inlet")]
        public string InletName = "pipe";
    }
}
