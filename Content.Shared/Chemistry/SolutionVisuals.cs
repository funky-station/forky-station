// SPDX-FileCopyrightText: 2023 Topy
// SPDX-FileCopyrightText: 2023 Moony
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
    [Serializable, NetSerializable]
    public enum SolutionContainerVisuals : byte
    {
        Color,
        FillFraction,
        BaseOverride,
        SolutionName
    }

    public enum SolutionContainerLayers : byte
    {
        Fill,
        Base,
        Overlay
    }
}
