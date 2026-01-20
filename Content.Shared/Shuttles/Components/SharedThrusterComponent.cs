// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Components
{
    [Serializable, NetSerializable]
    public enum ThrusterVisualState : byte
    {
        State,
        Thrusting,
    }
}
