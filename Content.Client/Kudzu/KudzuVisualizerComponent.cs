// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Jessica M
// SPDX-License-Identifier: MIT

namespace Content.Client.Kudzu
{
    [RegisterComponent]
    public sealed partial class KudzuVisualsComponent : Component
    {
        [DataField("layer")]
        public int Layer { get; private set; } = 0;
    }

}
