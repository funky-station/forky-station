// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Willhelm53
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Fluids
{
    [Serializable, NetSerializable]
    public enum PuddleVisuals : byte
    {
        CurrentVolume,
        SolutionColor,
    }
}
