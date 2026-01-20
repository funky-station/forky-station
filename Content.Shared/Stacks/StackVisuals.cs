// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Stacks
{
    [Serializable, NetSerializable]
    public enum StackVisuals : byte
    {
        /// <summary>
        /// The amount of elements in the stack
        /// </summary>
        Actual,
        /// <summary>
        /// The total amount of elements in the stack. If unspecified, the visualizer assumes
        /// it's StackComponent.LayerStates.Count
        /// </summary>
        MaxCount,
        Hide
    }
}
