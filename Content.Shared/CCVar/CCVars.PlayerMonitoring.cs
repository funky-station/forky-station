using Content.Shared.Administration.Logs;
using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    /// Maximum detail rows returned per EUI page.
    /// </summary>
    public static readonly CVarDef<int> MonitoringLogDetailPageSize =
        CVarDef.Create("monitoring.log_detail_page_size", 200, CVar.SERVERONLY);

    /// <summary>
    /// Maximum days selectable in the monitoring EUI (server clamps requests).
    /// </summary>
    public static readonly CVarDef<int> MonitoringLogMaxQueryDays =
        CVarDef.Create("monitoring.log_max_query_days", 90, CVar.SERVERONLY);

    /// <summary>
    /// When true, mid-round crew exits tracked for monitoring also emit PlayerMonitoringMidroundExit admin log lines (disconnect path).
    /// </summary>
    public static readonly CVarDef<bool> MonitoringAdminLogMirrorEnabled =
        CVarDef.Create("monitoring.admin_log_mirror_enabled", false, CVar.SERVERONLY);

    /// <summary>
    /// Prototype id (<see cref="PlayerMonitoringGhostWatchlistPrototype"/>) listing ghost-role entity prototypes to log when claimed.
    /// </summary>
    public static readonly CVarDef<string> MonitoringGhostWatchlistPrototype =
        CVarDef.Create("monitoring.ghost_watchlist_prototype", "AdminGhostRoleWatchlist", CVar.SERVERONLY);

    /// <summary>
    /// When true, at round restart scan admin logs for players with long idle gaps and write monitoring rows.
    /// </summary>
    public static readonly CVarDef<bool> MonitoringAfkFromAdminLogsEnabled =
        CVarDef.Create("monitoring.afk_from_admin_logs_enabled", true, CVar.SERVERONLY);

    /// <summary>
    /// Minimum longest idle gap (minutes between round timeline boundaries / attributed admin logs) to flag a player.
    /// </summary>
    public static readonly CVarDef<float> MonitoringAfkAdminLogsMinMinutes =
        CVarDef.Create("monitoring.afk_admin_logs_min_minutes", 60f, CVar.SERVERONLY);

    /// <summary>
    /// Minutes after round start after which a monitored exit is no longer flagged as early leave in the EUI.
    /// Values zero or negative fall back to the built-in default (2 minutes).
    /// </summary>
    public static readonly CVarDef<float> MonitoringEarlyLeaveMinutesRound =
        CVarDef.Create("monitoring.early_leave_minutes_round", 1f, CVar.SERVERONLY);
}
