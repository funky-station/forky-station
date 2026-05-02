using System.Text.Json;
using Content.Shared._Funkystation.Administration.Logs;
using Content.Shared.Database;

namespace Content.Server._Funkystation.Database;

/// <summary>
/// One row for the player monitoring EUI (materialized from <c>admin_log</c> / <c>admin_log_player</c>).
/// </summary>
public sealed class PlayerMonitoringLogView
{
    public int SurrogateId => HashCode.Combine(RoundId, LogId);

    public int RoundId { get; init; }
    public int LogId { get; init; }
    public DateTime Date { get; init; }
    public PlayerMonitoringEventType EventType { get; init; }
    public string DisplayUserName { get; init; } = string.Empty;

    /// <summary>Payload for detail columns (admin log JSON or merged monitoring JSON).</summary>
    public JsonDocument? DetailJson { get; init; }
}

public sealed class PlayerMonitoringQueryResult
{
    public required IReadOnlyList<PlayerMonitoringLogView> Page { get; init; }

    public bool HasNext { get; init; }

    public int FlaggedRoundsDenominator { get; init; }

    public int RoundsPlayedDenominator { get; init; }

    public required IReadOnlyDictionary<PlayerMonitoringEventType, PlayerMonitoringSummaryRow> Summary { get; init; }
}

public sealed class PlayerMonitoringSummaryRow
{
    public int Count { get; init; }
    public int DistinctRounds { get; init; }
}

public sealed record PlayerMonitoringLongAfkAdminLogsEntry(Guid UserId, string LastSeenUserName, double MaxIdleMinutes);

/// <summary>
/// Per UTC calendar day: elapsed time from earliest to latest successful connection log timestamp (plus last-seen when in range).
/// </summary>
public sealed class PlayerMonitoringDailyPlayDay
{
    public required string UtcDateIso { get; init; }
    public double SpanHours { get; init; }
}

public sealed class PlayerMonitoringDailyPlayStats
{
    public required List<PlayerMonitoringDailyPlayDay> Days { get; init; }
    public double TotalSpanHours { get; init; }
    public double AverageSpanHours { get; init; }
    public int ActiveDayCount { get; init; }
}

/// <summary>
/// Maps admin log rows to monitoring event types for UI aggregation.
/// </summary>
public static class PlayerMonitoringLogMappings
{
    public static PlayerMonitoringEventType ResolveEventType(LogType type, JsonDocument json, string message)
    {
        switch (type)
        {
            case LogType.PlayerMonitoring:
                if (json.RootElement.TryGetProperty("kind", out var k) && k.TryGetInt32(out var ki))
                    return (PlayerMonitoringEventType)ki;
                return default;

            case LogType.PlayerMonitoringMidroundExit:
                return PlayerMonitoringEventType.MidroundExitJobNonAntag;

            case LogType.PlayerMonitoringReconnectObserver:
                return PlayerMonitoringEventType.ReconnectDumpedToObserver;

            case LogType.Action when message.Contains("entered into cryostorage", StringComparison.OrdinalIgnoreCase):
                return PlayerMonitoringEventType.MidroundExitJobNonAntag;

            case LogType.GhostRoleTaken:
                return PlayerMonitoringEventType.HighValueGhostRoleTaken;

            default:
                return default;
        }
    }

}
