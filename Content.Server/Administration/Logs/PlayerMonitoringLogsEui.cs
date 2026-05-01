// SPDX-FileCopyrightText: 2026 Space Wizards Federation
// SPDX-License-Identifier: MIT

using System.Text.Json;
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Server.GameTicking;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Eui;
using Robust.Shared.Configuration;

namespace Content.Server.Administration.Logs;

public sealed class PlayerMonitoringLogsEui : BaseEui
{
    [Dependency] private readonly IAdminManager _adminManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IServerDbManager _db = default!;

    private int _pageOffset;
    private Guid? _activeUserId;
    private DateTime _rangeStart;
    private int _pageSize = 200;
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
            MaxDays = Math.Max(1, maxDays),
            DefaultDenominatorMode = PlayerMonitoringDenominatorMode.FlaggedRounds
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
                        RangeStartUtc = DateTime.UtcNow
                    });
                    return;
                }

                var days = Math.Clamp(req.Days, 1, _cfg.GetCVar(CCVars.MonitoringLogMaxQueryDays));
                _rangeStart = DateTime.UtcNow.AddDays(-days);

                var result = await _db.GetPlayerMonitoringLogsAsync(
                    _activeUserId.Value,
                    _rangeStart,
                    _pageOffset,
                    _pageSize);

                _pageOffset += result.Page.Count;

                SendMessage(BuildQueryResult(result, replace: true));
                break;
            }
            case PlayerMonitoringLogsEuiMsg.NextQuery:
            {
                if (_activeUserId == null)
                    return;

                var result = await _db.GetPlayerMonitoringLogsAsync(
                    _activeUserId.Value,
                    _rangeStart,
                    _pageOffset,
                    _pageSize);

                _pageOffset += result.Page.Count;

                SendMessage(new PlayerMonitoringLogsEuiMsg.QueryResult
                {
                    Rows = MapRows(result.Page),
                    Replace = false,
                    HasNext = result.HasNext,
                    UserNotFound = false
                });
                break;
            }
        }
    }

    private PlayerMonitoringLogsEuiMsg.QueryResult BuildQueryResult(PlayerMonitoringQueryResult result, bool replace)
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

        return new PlayerMonitoringLogsEuiMsg.QueryResult
        {
            Rows = MapRows(result.Page),
            Replace = replace,
            HasNext = result.HasNext,
            UserNotFound = false,
            Summary = summary,
            FlaggedRoundsDenominator = flagged,
            RoundsPlayedDenominator = played,
            RangeStartUtc = _rangeStart
        };
    }

    private static List<PlayerMonitoringDetailRow> MapRows(IReadOnlyList<AdminPlayerMonitoringLog> page)
    {
        var list = new List<PlayerMonitoringDetailRow>(page.Count);
        foreach (var r in page)
        {
            var row = new PlayerMonitoringDetailRow
            {
                Id = r.Id,
                Utc = r.Date,
                RoundId = r.RoundId,
                EventType = (PlayerMonitoringEventType)r.EventType,
                DisplayUserName = r.PlayerLastSeenUserName
            };

            if (r.Details != null)
            {
                var root = r.Details.RootElement;
                if (root.TryGetProperty("job", out var j))
                    row.Job = j.GetString();
                if (root.TryGetProperty("station", out var s))
                    row.Station = s.GetString();
                if (root.TryGetProperty("sub_reason", out var sr))
                    row.SubReason = sr.GetString();
                if (root.TryGetProperty("exit_kind", out var ek))
                    row.ExitKind = ek.GetString();
                if (root.TryGetProperty("disconnect_reason", out var dr))
                    row.DisconnectReason = dr.GetString();
                if (root.TryGetProperty("redial_flag", out var rf) &&
                    (rf.ValueKind == JsonValueKind.True || rf.ValueKind == JsonValueKind.False))
                    row.RedialFlag = rf.GetBoolean();
                if (root.TryGetProperty("minutes_in_round", out var mir) && mir.TryGetDouble(out var md))
                    row.MinutesInRound = md;
                if (root.TryGetProperty("minutes_since_round_start", out var mrs) && mrs.TryGetDouble(out var ms))
                    row.MinutesSinceRoundStart = ms;
                if (root.TryGetProperty("watched_entity_prototype", out var wep))
                    row.WatchedGhostEntityPrototype = wep.GetString();
                else if (root.TryGetProperty("high_value_kind", out var hv))
                    row.WatchedGhostEntityPrototype = hv.GetString();
                if (root.TryGetProperty("ghost_role_name", out var grn))
                    row.GhostRoleName = grn.GetString();
                if (root.TryGetProperty("max_idle_minutes", out var mim) && mim.TryGetDouble(out var afkMax))
                    row.AfkMaxIdleMinutes = afkMax;
                if (root.TryGetProperty("threshold_minutes", out var thr) && thr.TryGetDouble(out var afkThr))
                    row.AfkThresholdMinutes = afkThr;
            }

            list.Add(row);
        }

        return list;
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
