using System.Text.Json;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Logs;

public sealed class PlayerMonitoringLogsEui : BaseEui
{
    private const float EarlyLeaveMinutesRound = 2f;

    [Dependency] private readonly IAdminManager _adminManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;

    private int _pageOffset;
    private Guid? _activeUserId;
    private DateTime _rangeStart;
    private int _pageSize = 200;

    public PlayerMonitoringLogsEui()
    {
        IoCManager.InjectDependencies(this);
        _sawmill = _logManager.GetSawmill("admin.player-monitoring");
    }

    public override void Opened()
    {
        base.Opened();
        _adminManager.OnPermsChanged += OnPermsChanged;
        _pageSize = Math.Clamp(_cfg.GetCVar(CCVars.MonitoringLogDetailPageSize), 1, 500);
        StateDirty();
    }

    public override EuiStateBase GetNewState()
    {
        var maxDays = _cfg.GetCVar(CCVars.MonitoringLogMaxQueryDays);
        return new PlayerMonitoringLogsEuiState
        {
            DefaultDays = 7,
            MaxDays = Math.Max(1, maxDays)
        };
    }

    public override async void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (!_adminManager.HasAdminFlag(Player, AdminFlags.Logs))
            return;

        switch (msg)
        {
            case PlayerMonitoringLogsEuiMsg.RequestQuery req:
            {
                _pageOffset = 0;
                _activeUserId = req.UserId;
                if (_activeUserId == null && !string.IsNullOrWhiteSpace(req.UserNameExact))
                {
                    _activeUserId = await _db.ResolveUserIdByExactNameAsync(req.UserNameExact.Trim());
                }

                if (_activeUserId == null)
                {
                    SendMessage(new PlayerMonitoringLogsEuiMsg.QueryResult
                    {
                        Rows = new List<PlayerMonitoringDetailRow>(),
                        Replace = true,
                        HasNext = false,
                        UserNotFound = true,
                        Summary = null,
                        FlaggedRoundsDenominator = null,
                        RoundsPlayedDenominator = null,
                        RangeStartUtc = DateTime.UtcNow,
                        DailyPlayByUtcDay = null,
                        TotalDailyPlaySpanHours = 0,
                        AverageDailyPlaySpanHours = 0,
                        DailyPlayActiveDayCount = 0
                    });
                    return;
                }

                var days = Math.Clamp(req.Days, 1, _cfg.GetCVar(CCVars.MonitoringLogMaxQueryDays));
                _rangeStart = DateTime.UtcNow.AddDays(-days);

                var ghostFilter = BuildGhostRoleTakenFilter();
                var untilUtc = DateTime.UtcNow;
                var logsTask = _db.GetPlayerMonitoringLogsAsync(
                    _activeUserId.Value,
                    _rangeStart,
                    _pageOffset,
                    _pageSize,
                    ghostFilter);
                var dailyTask = _db.GetPlayerMonitoringDailyPlayStatsAsync(
                    _activeUserId.Value,
                    _rangeStart,
                    untilUtc);
                await Task.WhenAll(logsTask, dailyTask);
                var result = await logsTask;
                var dailyPlay = await dailyTask;

                _pageOffset += result.Page.Count;

                SendMessage(BuildQueryResult(result, replace: true, dailyPlay));
                break;
            }
            case PlayerMonitoringLogsEuiMsg.NextQuery:
            {
                if (_activeUserId == null)
                    return;

                var ghostFilter = BuildGhostRoleTakenFilter();
                var result = await _db.GetPlayerMonitoringLogsAsync(
                    _activeUserId.Value,
                    _rangeStart,
                    _pageOffset,
                    _pageSize,
                    ghostFilter);

                _pageOffset += result.Page.Count;

                SendMessage(new PlayerMonitoringLogsEuiMsg.QueryResult
                {
                    Rows = MapRows(result.Page, ResolveEarlyLeaveThresholdMinutes()),
                    Replace = false,
                    HasNext = result.HasNext,
                    UserNotFound = false
                });
                break;
            }
        }
    }

    private Func<JsonDocument, bool>? BuildGhostRoleTakenFilter()
    {
        var watchIds = PlayerMonitoringEuiWatchlist.BuildDirectWatchIds(_cfg, _proto, _sawmill);
        if (watchIds.Count == 0)
            return null;

        return json => PlayerMonitoringEuiWatchlist.JsonGhostMatchesWatchlist(json, watchIds, _proto);
    }

    private PlayerMonitoringLogsEuiMsg.QueryResult BuildQueryResult(
        PlayerMonitoringQueryResult result,
        bool replace,
        PlayerMonitoringDailyPlayStats dailyPlay)
    {
        var flagged = result.FlaggedRoundsDenominator;
        var played = result.RoundsPlayedDenominator;

        var summary = new Dictionary<PlayerMonitoringEventType, PlayerMonitoringLogsEuiMsg.PlayerMonitoringSummaryEntry>();
        foreach (var (type, row) in result.Summary)
        {
            var pf = flagged > 0 ? row.DistinctRounds / (double)flagged * 100.0 : 0;
            var pp = played > 0 ? row.DistinctRounds / (double)played * 100.0 : 0;
            summary[type] = new PlayerMonitoringLogsEuiMsg.PlayerMonitoringSummaryEntry
            {
                Count = row.Count,
                DistinctRounds = row.DistinctRounds,
                PercentFlaggedRounds = pf,
                PercentRoundsPlayed = pp
            };
        }

        var dailyList = new List<PlayerMonitoringDailyPlayDayEntry>(dailyPlay.Days.Count);
        foreach (var d in dailyPlay.Days)
        {
            dailyList.Add(new PlayerMonitoringDailyPlayDayEntry
            {
                UtcDate = d.UtcDateIso,
                SpanHours = d.SpanHours
            });
        }

        return new PlayerMonitoringLogsEuiMsg.QueryResult
        {
            Rows = MapRows(result.Page, ResolveEarlyLeaveThresholdMinutes()),
            Replace = replace,
            HasNext = result.HasNext,
            UserNotFound = false,
            Summary = summary,
            FlaggedRoundsDenominator = flagged,
            RoundsPlayedDenominator = played,
            RangeStartUtc = _rangeStart,
            DailyPlayByUtcDay = dailyList,
            TotalDailyPlaySpanHours = dailyPlay.TotalSpanHours,
            AverageDailyPlaySpanHours = dailyPlay.AverageSpanHours,
            DailyPlayActiveDayCount = dailyPlay.ActiveDayCount
        };
    }

    private float ResolveEarlyLeaveThresholdMinutes()
    {
        var v = _cfg.GetCVar(CCVars.MonitoringEarlyLeaveMinutesRound);
        if (float.IsNaN(v) || float.IsInfinity(v) || v <= 0f)
            return EarlyLeaveMinutesRound;
        return v;
    }

    private static List<PlayerMonitoringDetailRow> MapRows(IReadOnlyList<PlayerMonitoringLogView> page, float earlyLeaveThresholdMinutes)
    {
        var list = new List<PlayerMonitoringDetailRow>(page.Count);
        foreach (var r in page)
        {
            var row = new PlayerMonitoringDetailRow
            {
                Id = r.SurrogateId,
                Utc = r.Date,
                RoundId = r.RoundId,
                EventType = r.EventType,
                DisplayUserName = r.DisplayUserName
            };

            if (r.DetailJson == null)
            {
                list.Add(row);
                continue;
            }

            var root = r.DetailJson.RootElement;
            if (root.TryGetProperty("job", out var j) && j.ValueKind == JsonValueKind.String)
                row.Job = j.GetString();
            if (root.TryGetProperty("station", out var s) && s.ValueKind == JsonValueKind.String)
                row.Station = s.GetString();
            if (root.TryGetProperty("sub_reason", out var sr) && sr.ValueKind == JsonValueKind.String)
                row.SubReason = sr.GetString();
            else if (root.TryGetProperty("subReason", out var sr2) && sr2.ValueKind == JsonValueKind.String)
                row.SubReason = sr2.GetString();
            if (root.TryGetProperty("exit_kind", out var ek) && ek.ValueKind == JsonValueKind.String)
                row.ExitKind = ek.GetString();
            if (root.TryGetProperty("disconnect_reason", out var dr) && dr.ValueKind == JsonValueKind.String)
                row.DisconnectReason = dr.GetString();
            if (root.TryGetProperty("redial_flag", out var rf) &&
                (rf.ValueKind == JsonValueKind.True || rf.ValueKind == JsonValueKind.False))
                row.RedialFlag = rf.GetBoolean();
            if (root.TryGetProperty("minutes_in_round", out var mir) && mir.TryGetDouble(out var md))
                row.MinutesInRound = md;
            if (root.TryGetProperty("minutes_since_round_start", out var mrs) && mrs.TryGetDouble(out var ms))
                row.MinutesSinceRoundStart = ms;
            if (root.TryGetProperty("ghostRoleEntityPrototype", out var grp) && grp.ValueKind == JsonValueKind.String)
                row.WatchedGhostEntityPrototype = grp.GetString();
            else if (root.TryGetProperty("watched_entity_prototype", out var wep) && wep.ValueKind == JsonValueKind.String)
                row.WatchedGhostEntityPrototype = wep.GetString();
            else if (root.TryGetProperty("high_value_kind", out var hv) && hv.ValueKind == JsonValueKind.String)
                row.WatchedGhostEntityPrototype = hv.GetString();
            if (root.TryGetProperty("ghost_role_name", out var grn) && grn.ValueKind == JsonValueKind.String)
                row.GhostRoleName = grn.GetString();
            else if (root.TryGetProperty("roleName", out var roleName) && roleName.ValueKind == JsonValueKind.String)
                row.GhostRoleName = roleName.GetString();
            if (root.TryGetProperty("max_idle_minutes", out var mim) && mim.TryGetDouble(out var afkMax))
                row.AfkMaxIdleMinutes = afkMax;
            if (root.TryGetProperty("threshold_minutes", out var thr) && thr.TryGetDouble(out var afkThr))
                row.AfkThresholdMinutes = afkThr;

            row.EarlyLeave = IsEarlyLeave(row, earlyLeaveThresholdMinutes);

            list.Add(row);
        }

        return list;
    }

    private static bool IsEarlyLeave(PlayerMonitoringDetailRow row, float thresholdMinutes)
    {
        switch (row.EventType)
        {
            case PlayerMonitoringEventType.MidroundExitJobNonAntag:
                if (row.MinutesSinceRoundStart is { } mrs)
                    return mrs < thresholdMinutes;
                if (row.MinutesInRound is { } mir)
                    return mir < thresholdMinutes;
                return false;
            case PlayerMonitoringEventType.ObserverOnlyMidroundDisconnect:
                return row.MinutesSinceRoundStart is { } mo && mo < thresholdMinutes;
            default:
                return false;
        }
    }

    private void OnPermsChanged(AdminPermsChangedEventArgs args)
    {
        if (args.Player == Player && !_adminManager.HasAdminFlag(Player, AdminFlags.Logs))
            Close();
    }

    public override void Closed()
    {
        base.Closed();
        _adminManager.OnPermsChanged -= OnPermsChanged;
    }
}

internal static class PlayerMonitoringEuiWatchlist
{
    public static HashSet<string> BuildDirectWatchIds(IConfigurationManager cfg, IPrototypeManager proto, ISawmill sawmill)
    {
        var set = new HashSet<string>();
        var listId = cfg.GetCVar(CCVars.MonitoringGhostWatchlistPrototype);
        if (string.IsNullOrWhiteSpace(listId))
            return set;

        if (!proto.TryIndex<PlayerMonitoringGhostWatchlistPrototype>(listId, out var list))
        {
            sawmill.Warning($"Player monitoring ghost watchlist prototype '{listId}' not found.");
            return set;
        }

        foreach (var id in list.GhostRoles)
            set.Add(id.Id);

        return set;
    }

    public static bool JsonGhostMatchesWatchlist(JsonDocument json, HashSet<string> directIds, IPrototypeManager proto)
    {
        if (directIds.Count == 0)
            return false;

        if (!json.RootElement.TryGetProperty("ghostRoleEntityPrototype", out var p) ||
            p.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        var id = p.GetString();
        if (string.IsNullOrEmpty(id) || !proto.TryIndex<EntityPrototype>(id, out var ent))
            return false;

        return IsEntityWatchlisted(ent, directIds, proto);
    }

    private static bool IsEntityWatchlisted(EntityPrototype entityProto, HashSet<string> directIds, IPrototypeManager proto)
    {
        if (directIds.Contains(entityProto.ID))
            return true;

        foreach (var a in EnumerateAncestorPrototypeIds(entityProto, proto))
        {
            if (directIds.Contains(a))
                return true;
        }

        return false;
    }

    private static IEnumerable<string> EnumerateAncestorPrototypeIds(EntityPrototype proto, IPrototypeManager prototypes)
    {
        var queue = new Queue<string>();
        var visited = new HashSet<string>();
        foreach (var parent in proto.Parents ?? [])
            queue.Enqueue(parent);

        while (queue.Count > 0)
        {
            var pid = queue.Dequeue();
            if (!visited.Add(pid))
                continue;

            yield return pid;

            if (!prototypes.TryIndex<EntityPrototype>(pid, out var parent))
                continue;

            foreach (var gp in parent.Parents ?? [])
                queue.Enqueue(gp);
        }
    }
}
