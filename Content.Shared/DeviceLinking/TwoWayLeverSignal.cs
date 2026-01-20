// SPDX-FileCopyrightText: 2023 Julian Giebel
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Jack Fox
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.DeviceLinking
{
    [Serializable, NetSerializable]
    public enum TwoWayLeverVisuals : byte
    {
        State
    }

    [Serializable, NetSerializable]
    public enum TwoWayLeverState : byte
    {
        Middle,
        Right,
        Left
    }
}
