// SPDX-FileCopyrightText: 2026 Space Wizards Federation
// SPDX-License-Identifier: MIT

namespace Content.Shared.Administration.Logs;

/// <summary>
/// Stable numeric values for <c>admin_player_monitoring_log.event_type</c>. Do not reorder.
/// </summary>
public enum PlayerMonitoringEventType : int
{
    /// <summary>
    /// No job at spawn; lobby enabled — player remains in lobby.
    /// </summary>
    NoJobWaitLobbyRoundStart = 0,

    /// <summary>
    /// No job at spawn; lobby disabled — player becomes observer.
    /// </summary>
    NoJobBecameObserver = 1,

    /// <summary>
    /// Reconnected with a mind but no valid attached entity (or attach failed) — dumped to observer.
    /// </summary>
    ReconnectDumpedToObserver = 2,

    /// <summary>
    /// In-round exit (disconnect or cryo) while having a station job snapshot and not an antagonist.
    /// </summary>
    MidroundExitJobNonAntag = 3,

    /// <summary>
    /// Player returned to <see cref="SessionStatus.InGame"/> in the same round after a monitored leave.
    /// </summary>
    SameRoundReconnect = 4,

    /// <summary>
    /// Disconnected mid-round while observing / ghosting without ever having a crew job spawn snapshot this round.
    /// </summary>
    ObserverOnlyMidroundDisconnect = 5,

    /// <summary>
    /// Took a ghost role on an entity prototype listed in <see cref="PlayerMonitoringGhostWatchlistPrototype"/>.
    /// </summary>
    HighValueGhostRoleTaken = 6,

    /// <summary>
    /// At round end: longest gap between round start, attributed admin logs, and round end exceeded the configured threshold.
    /// </summary>
    LongAfkFromAdminLogsRoundEnd = 7,
}
