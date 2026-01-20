// SPDX-FileCopyrightText: 2023 daerSeebaer
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Visuals
{
    [Serializable, NetSerializable]
    public enum GasVolumePumpVisuals : byte
    {
        State,
    }

    [Serializable, NetSerializable]
    public enum GasVolumePumpState : byte
    {
        Off,
        On,
        Blocked,
    }
}
