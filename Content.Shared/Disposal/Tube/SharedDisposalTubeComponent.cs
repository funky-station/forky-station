// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Jack Fox
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Disposal.Components
{
    [Serializable, NetSerializable]
    public enum DisposalTubeVisuals
    {
        VisualState
    }

    [Serializable, NetSerializable]
    public enum DisposalTubeVisualState
    {
        Free = 0,
        Anchored,
    }
}
