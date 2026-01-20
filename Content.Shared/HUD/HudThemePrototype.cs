// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.HUD
{
    [Prototype]
    public sealed partial class HudThemePrototype : IPrototype, IComparable<HudThemePrototype>
    {
        [DataField("name", required: true)]
        public string Name { get; private set; } = string.Empty;

        [IdDataField]
        public string ID { get; private set; } = string.Empty;

        [DataField("path", required: true)]
        public string Path { get; private set; } = string.Empty;

        /// <summary>
        /// An order for the themes to be displayed in the UI
        /// </summary>
        [DataField]
        public int Order = 0;

        public int CompareTo(HudThemePrototype? other)
        {
            return Order.CompareTo(other?.Order);
        }
    }
}
