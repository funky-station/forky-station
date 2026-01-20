// SPDX-FileCopyrightText: 2024 Jezithyr
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Atmos;

namespace Content.Server.Atmos.Piping.Binary.Components
{
    [RegisterComponent]
    public sealed partial class GasPortComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("pipe")]
        public string PipeName { get; set; } = "connected";

        [ViewVariables(VVAccess.ReadOnly)]
        public GasMixture Buffer { get; } = new();
    }
}
