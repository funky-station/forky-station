// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2022 ScalyChimp
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2019 Injazz
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Trigger;

[Serializable, NetSerializable]
public enum ProximityTriggerVisuals : byte
{
    Off,
    Inactive,
    Active,
}

[Serializable, NetSerializable]
public enum ProximityTriggerVisualState : byte
{
    State,
}

[Serializable, NetSerializable]
public enum TriggerVisuals : byte
{
    VisualState,
}

[Serializable, NetSerializable]
public enum TriggerVisualState : byte
{
    Primed,
    Unprimed,
}
