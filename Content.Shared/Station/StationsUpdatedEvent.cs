// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Station;

[NetSerializable, Serializable]
public sealed class StationsUpdatedEvent : EntityEventArgs
{
    public readonly List<(string Name, NetEntity Entity)> Stations;

    public StationsUpdatedEvent(List<(string Name, NetEntity Entity)> stations)
    {
        Stations = stations;
    }
}
