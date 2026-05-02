using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Administration.Logs;

[Serializable, NetSerializable]
public sealed class PlayerMonitoringLogsEuiState : EuiStateBase
{
    public PlayerMonitoringLogsEuiState()
    {
    }

    public int DefaultDays { get; init; } = 7;

    public int MaxDays { get; init; } = 90;
}

/// <summary>
/// UTC calendar day presence span derived from connection logs (see server query).
/// </summary>
[Serializable, NetSerializable]
public sealed class PlayerMonitoringDailyPlayDayEntry
{
    public string UtcDate { get; set; } = string.Empty;
    public double SpanHours { get; set; }
}

[Serializable, NetSerializable]
public sealed class PlayerMonitoringDetailRow
{
    public int Id { get; set; }
    public DateTime Utc { get; set; }
    public int RoundId { get; set; }
    public PlayerMonitoringEventType EventType { get; set; }
    public string DisplayUserName { get; set; } = string.Empty;
    public string? Job { get; set; }
    public string? Station { get; set; }
    public string? SubReason { get; set; }
    public string? ExitKind { get; set; }
    public string? DisconnectReason { get; set; }
    public bool? RedialFlag { get; set; }
    public double? MinutesInRound { get; set; }

    /// <summary>
    /// From details JSON when present: matched watchlist entity prototype id.
    /// </summary>
    public string? WatchedGhostEntityPrototype { get; set; }

    /// <summary>
    /// Localized ghost role title when present.
    /// </summary>
    public string? GhostRoleName { get; set; }

    /// <summary>
    /// Minutes since the round entered <c>InRound</c>, when present (observer-only disconnect).
    /// </summary>
    public double? MinutesSinceRoundStart { get; set; }

    /// <summary>
    /// From details when present (long AFK from admin logs scan).
    /// </summary>
    public double? AfkMaxIdleMinutes { get; set; }

    /// <summary>
    /// Threshold minutes used for the AFK scan when present.
    /// </summary>
    public double? AfkThresholdMinutes { get; set; }

    /// <summary>
    /// True when the exit occurred within the configured early-leave window (minutes after round start).
    /// </summary>
    public bool EarlyLeave { get; set; }
}

public static class PlayerMonitoringLogsEuiMsg
{
    [Serializable, NetSerializable]
    public sealed class RequestQuery : EuiMessageBase
    {
        public Guid? UserId { get; set; }
        public string? UserNameExact { get; set; }
        public int Days { get; set; }
    }

    [Serializable, NetSerializable]
    public sealed class NextQuery : EuiMessageBase
    {
    }

    [Serializable, NetSerializable]
    public sealed class QueryResult : EuiMessageBase
    {
        public List<PlayerMonitoringDetailRow> Rows { get; set; } = new();
        public bool Replace { get; set; }
        public bool HasNext { get; set; }
        public bool UserNotFound { get; set; }

        /// <summary>
        /// Populated only when <see cref="Replace"/> is true.
        /// </summary>
        public Dictionary<PlayerMonitoringEventType, PlayerMonitoringSummaryEntry>? Summary { get; set; }

        public int? FlaggedRoundsDenominator { get; set; }
        public int? RoundsPlayedDenominator { get; set; }
        public DateTime RangeStartUtc { get; set; }

        /// <summary>
        /// Populated when <see cref="Replace"/> is true and the query succeeded. Sorted by <see cref="PlayerMonitoringDailyPlayDayEntry.UtcDate"/>.
        /// </summary>
        public List<PlayerMonitoringDailyPlayDayEntry>? DailyPlayByUtcDay { get; set; }

        public double TotalDailyPlaySpanHours { get; set; }
        public double AverageDailyPlaySpanHours { get; set; }
        public int DailyPlayActiveDayCount { get; set; }

        /// <summary>
        /// Set when the query failed (database error, malformed data). Details are not shown to clients.
        /// </summary>
        public string? QueryError { get; set; }
    }

    [Serializable, NetSerializable]
    public sealed class PlayerMonitoringSummaryEntry
    {
        public int Count { get; set; }
        public int DistinctRounds { get; set; }
        public double PercentFlaggedRounds { get; set; }
        public double PercentRoundsPlayed { get; set; }
    }
}
