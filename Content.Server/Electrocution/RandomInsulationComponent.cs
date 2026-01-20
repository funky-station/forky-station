// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

namespace Content.Server.Electrocution
{
    [RegisterComponent]
    public sealed partial class RandomInsulationComponent : Component
    {
        [DataField("list")]
        public float[] List = { 0f };
    }
}
