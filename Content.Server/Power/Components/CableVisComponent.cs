// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

namespace Content.Server.Power.Components
{
    [RegisterComponent]
    public sealed partial class CableVisComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("node", required:true)]
        public string Node;
    }
}
