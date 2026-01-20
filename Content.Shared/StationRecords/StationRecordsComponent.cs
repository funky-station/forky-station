// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-License-Identifier: MIT

namespace Content.Shared.StationRecords;

[Access(typeof(SharedStationRecordsSystem))]
[RegisterComponent]
public sealed partial class StationRecordsComponent : Component
{
    // Every single record in this station, by key.
    // Essentially a columnar database, but I really suck
    // at implementing that so
    [IncludeDataField]
    public StationRecordSet Records = new();
}
