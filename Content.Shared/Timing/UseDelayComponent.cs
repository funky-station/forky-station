// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Peter Wedder
// SPDX-FileCopyrightText: 2020 L.E.D
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Timing;

/// <summary>
/// Timer that creates a cooldown each time an object is activated/used.
/// Can support additional, separate cooldown timers on the object by passing a unique ID with the system methods.
/// </summary>
[RegisterComponent]
[NetworkedComponent]
[Access(typeof(UseDelaySystem))]
public sealed partial class UseDelayComponent : Component
{
    [DataField]
    public Dictionary<string, UseDelayInfo> Delays = [];

    /// <summary>
    /// Default delay time.
    /// </summary>
    /// <remarks>
    /// This is only used at MapInit and should not be expected
    /// to reflect the length of the default delay after that.
    /// Use <see cref="UseDelaySystem.TryGetDelayInfo"/> instead.
    /// </remarks>
    [DataField]
    public TimeSpan Delay = TimeSpan.FromSeconds(1);
}

[Serializable, NetSerializable]
public sealed class UseDelayComponentState : IComponentState
{
    public Dictionary<string, UseDelayInfo> Delays = new();
}

[Serializable, NetSerializable]
[DataDefinition]
public sealed partial class UseDelayInfo
{
    [DataField]
    public TimeSpan Length { get; set; }
    [DataField]
    public TimeSpan StartTime { get; set; }
    [DataField]
    public TimeSpan EndTime { get; set; }

    public UseDelayInfo(TimeSpan length, TimeSpan startTime = default, TimeSpan endTime = default)
    {
        Length = length;
        StartTime = startTime;
        EndTime = endTime;
    }
}
