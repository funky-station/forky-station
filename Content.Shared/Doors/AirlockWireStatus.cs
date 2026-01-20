// SPDX-FileCopyrightText: 2024 chromiumboy
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Doors
{
    [Serializable, NetSerializable]
    public enum AirlockWireStatus
    {
        PowerIndicator,
        BoltIndicator,
        BoltLightIndicator,
        AiControlIndicator,
        AiVisionIndicator,
        TimingIndicator,
        SafetyIndicator,
    }
}
