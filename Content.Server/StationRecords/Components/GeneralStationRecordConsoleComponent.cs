// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Artjom
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-License-Identifier: MIT

using Content.Server.StationRecords.Systems;
using Content.Shared.StationRecords;

namespace Content.Server.StationRecords.Components;

[RegisterComponent, Access(typeof(GeneralStationRecordConsoleSystem))]
public sealed partial class GeneralStationRecordConsoleComponent : Component
{
    /// <summary>
    /// Selected crewmember record id.
    /// Station always uses the station that owns the console.
    /// </summary>
    [DataField]
    public uint? ActiveKey;

    /// <summary>
    /// Qualities to filter a search by.
    /// </summary>
    [DataField]
    public StationRecordsFilter? Filter;

    /// <summary>
    /// Whether this Records Console is able to delete entries.
    /// </summary>
    [DataField]
    public bool CanDeleteEntries;
}
