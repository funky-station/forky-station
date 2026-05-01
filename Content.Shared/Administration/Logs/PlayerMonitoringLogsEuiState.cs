// SPDX-FileCopyrightText: 2026 Space Wizards Federation
// SPDX-License-Identifier: MIT

using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Logs;

[Serializable, NetSerializable]
public enum PlayerMonitoringDenominatorMode : byte
{
    FlaggedRounds = 0,
    RoundsPlayed = 1,
}

[Serializable, NetSerializable]
public sealed class PlayerMonitoringLogsEuiState : EuiStateBase
{
    public PlayerMonitoringLogsEuiState()
    {
    }

    public int DefaultDays { get; init; } = 7;

    public int MaxDays { get; init; } = 90;

    public PlayerMonitoringDenominatorMode DefaultDenominatorMode { get; init; } = PlayerMonitoringDenominatorMode.FlaggedRounds;
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
}

public static class PlayerMonitoringLogsEuiMsg
{
    [Serializable, NetSerializable]
    public sealed class RequestQuery : EuiMessageBase
    {
        public Guid? UserId { get; set; }
        public string? UserNameExact { get; set; }
        public int Days { get; set; }
        public PlayerMonitoringDenominatorMode DenominatorMode { get; set; }
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
