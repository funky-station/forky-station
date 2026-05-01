// SPDX-FileCopyrightText: 2026 Space Wizards Federation
// SPDX-License-Identifier: MIT

using Content.Shared.Administration.Logs;

namespace Content.Server.Database;

/// <summary>
/// Result of a paginated player-monitoring log query (one user, time range).
/// </summary>
public sealed class PlayerMonitoringQueryResult
{
    public required IReadOnlyList<AdminPlayerMonitoringLog> Page { get; init; }

    public bool HasNext { get; init; }

    public int FlaggedRoundsDenominator { get; init; }

    public int RoundsPlayedDenominator { get; init; }

    /// <summary>
    /// Per <see cref="PlayerMonitoringEventType"/> aggregate for the full filtered set (not only the page).
    /// </summary>
    public required IReadOnlyDictionary<PlayerMonitoringEventType, PlayerMonitoringSummaryRow> Summary { get; init; }
}

public sealed class PlayerMonitoringSummaryRow
{
    public int Count { get; init; }
    public int DistinctRounds { get; init; }
}
