// SPDX-FileCopyrightText: 2026 Space Wizards Federation
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Content.Server.Antag;
using Content.Server.Database;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Events;
using Content.Server.Ghost.Roles;
using Content.Server.Ghost.Roles.Components;
using System.Threading.Tasks;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Content.Shared.Mind;
using Content.Shared.Roles;
using Content.Shared.Roles.Components;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.Administration.Logs;

/// <summary>
/// Queues player-monitoring DB rows; subscribes to game/session events. Observation-only (never mutates gameplay state).
/// </summary>
public sealed class PlayerMonitoringLogSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly GameTicker _ticker = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly IServerNetManager _net = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly SharedRoleSystem _roles = default!;
    [Dependency] private readonly IAdminLogManager _adminLog = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    private ISawmill _sawmill = default!;

    private readonly Queue<PendingRow> _queue = new();
    private readonly object _queueLock = new();

    /// <summary>
    /// Per-user job snapshot for the current round (cleared on round restart).
    /// </summary>
    private readonly Dictionary<NetUserId, (string JobId, TimeSpan SpawnTime)> _spawnedJob = new();

    private readonly HashSet<(NetUserId User, PlayerMonitoringEventType Type)> _loggedNoJob = new();

    private readonly HashSet<NetUserId> _leftThisRound = new();

    /// <summary>
    /// Mid-round disconnect row: first signal owns the pending merge target until flushed.
    /// </summary>
    private readonly Dictionary<NetUserId, PendingRow> _midroundDisconnectPending = new();

    /// <summary>
    /// Game timing mark when the round is live (<see cref="GameRunLevel.InRound"/>); used for observer-only disconnect detail.
    /// </summary>
    private TimeSpan? _roundInRoundSince;

    private readonly HashSet<string> _ghostWatchPrototypeIds = new();

    private int _drops;
    private volatile int _flushGate;
    private TimeSpan _nextFlush;
    private TimeSpan _nextPruneAttempt;
    private bool _pruneScheduledThisRound;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("admin.player_monitoring");

        SubscribeLocalEvent<NoJobsAvailableSpawningEvent>(OnNoJobs);
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnSpawnComplete);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundCleanup);
        SubscribeLocalEvent<GameRunLevelChangedEvent>(OnGameRunLevelChanged);
        SubscribeLocalEvent<GhostRoleComponent, TakeGhostRoleEvent>(OnGhostRoleTakenForMonitoring,
            after: [typeof(GhostRoleSystem), typeof(AntagSelectionSystem)]);

        _prototype.PrototypesReloaded += OnPrototypesReloaded;
        _cfg.OnValueChanged(CCVars.MonitoringGhostWatchlistPrototype, _ => RebuildGhostWatchlistCache(), invokeImmediately: true);

        _playerManager.PlayerStatusChanged += OnPlayerStatusChanged;
        _net.Disconnect += OnNetDisconnect;

        _nextFlush = _timing.CurTime;
        _nextPruneAttempt = _timing.CurTime;
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _prototype.PrototypesReloaded -= OnPrototypesReloaded;
        _playerManager.PlayerStatusChanged -= OnPlayerStatusChanged;
        _net.Disconnect -= OnNetDisconnect;
    }

    private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        RebuildGhostWatchlistCache();
    }

    private void RebuildGhostWatchlistCache()
    {
        _ghostWatchPrototypeIds.Clear();

        var listId = _cfg.GetCVar(CCVars.MonitoringGhostWatchlistPrototype);
        if (string.IsNullOrWhiteSpace(listId))
            return;

        if (!_prototype.TryIndex<PlayerMonitoringGhostWatchlistPrototype>(listId, out var wl))
        {
            _sawmill.Warning($"Player monitoring ghost watchlist prototype '{listId}' not found.");
            return;
        }

        foreach (var id in wl.GhostRoles)
            _ghostWatchPrototypeIds.Add(id.Id);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_timing.CurTime < _nextFlush)
            return;

        _nextFlush = _timing.CurTime + TimeSpan.FromMilliseconds(500);
        _ = Task.Run(FlushQueueAsync);
    }

    private async Task FlushQueueAsync()
    {
        if (Interlocked.Exchange(ref _flushGate, 1) != 0)
            return;

        try
        {
            List<PendingRow>? batch = null;
            lock (_queueLock)
            {
                if (_queue.Count == 0)
                    return;

                var maxBatch = 64;
                batch = new List<PendingRow>(Math.Min(maxBatch, _queue.Count));
                while (_queue.Count > 0 && batch.Count < maxBatch)
                {
                    batch.Add(_queue.Dequeue());
                }
            }

            if (batch == null || batch.Count == 0)
                return;

            var entities = new List<AdminPlayerMonitoringLog>(batch.Count);
            foreach (var row in batch)
            {
                try
                {
                    entities.Add(new AdminPlayerMonitoringLog
                    {
                        RoundId = row.RoundId,
                        PlayerUserId = row.UserId,
                        PlayerLastSeenUserName = row.LastSeenUserName,
                        EventType = (int)row.Type,
                        Date = row.Utc,
                        Details = row.Details
                    });
                    row.Details = null; // ownership transferred to entity
                }
                catch (Exception e)
                {
                    _sawmill.Error($"Error building monitoring row: {e}");
                    row.Details?.Dispose();
                }
            }

            if (entities.Count == 0)
                return;

            try
            {
                await _db.AddPlayerMonitoringLogsAsync(entities);
                foreach (var src in batch)
                {
                    if (src.Type == PlayerMonitoringEventType.MidroundExitJobNonAntag)
                        _midroundDisconnectPending.Remove(new NetUserId(src.UserId));
                }
            }
            catch (Exception e)
            {
                _sawmill.Error($"Failed to flush player monitoring logs: {e}");
                foreach (var e2 in entities)
                {
                    e2.Details?.Dispose();
                }
            }
        }
        finally
        {
            Interlocked.Exchange(ref _flushGate, 0);
        }
    }

    private void OnRoundCleanup(RoundRestartCleanupEvent ev)
    {
        // Still the round that is ending; IncrementRoundNumber runs after this event in RestartRound.
        var endingRoundId = _ticker.RoundId;

        _spawnedJob.Clear();
        _loggedNoJob.Clear();
        _leftThisRound.Clear();
        _midroundDisconnectPending.Clear();
        _roundInRoundSince = null;
        _pruneScheduledThisRound = false;

        SchedulePrune();
        _ = Task.Run(() => ScanLongAfkFromAdminLogsAtRoundEndAsync(endingRoundId));
    }

    private async Task ScanLongAfkFromAdminLogsAtRoundEndAsync(int endingRoundId)
    {
        try
        {
            if (!_cfg.GetCVar(CCVars.MonitoringAfkFromAdminLogsEnabled))
                return;

            if (endingRoundId == 0)
                return;

            var threshold = _cfg.GetCVar(CCVars.MonitoringAfkAdminLogsMinMinutes);
            if (threshold <= 0f)
                return;

            await _adminLog.FlushInRoundAdminLogsAsync();

            var roundEndUtc = DateTime.UtcNow;
            var flagged = await _db.QueryPlayersLongAfkFromAdminLogsAsync(endingRoundId, roundEndUtc, threshold);

            foreach (var entry in flagged)
            {
                var details = JsonSerializer.SerializeToDocument(new
                {
                    max_idle_minutes = entry.MaxIdleMinutes,
                    threshold_minutes = (double)threshold,
                    analysis_end_utc = roundEndUtc
                });

                TryEnqueue(new PendingRow
                {
                    UserId = entry.UserId,
                    LastSeenUserName = entry.LastSeenUserName,
                    RoundId = endingRoundId,
                    Type = PlayerMonitoringEventType.LongAfkFromAdminLogsRoundEnd,
                    Utc = roundEndUtc,
                    Details = details
                });
            }
        }
        catch (Exception e)
        {
            _sawmill.Error($"Long AFK (admin logs) round-end scan: {e}");
        }
    }

    private void OnGameRunLevelChanged(GameRunLevelChangedEvent ev)
    {
        if (ev.New == GameRunLevel.InRound)
            _roundInRoundSince = _timing.CurTime;
        else if (ev.New != GameRunLevel.InRound)
            _roundInRoundSince = null;
    }

    private void OnGhostRoleTakenForMonitoring(Entity<GhostRoleComponent> ent, ref TakeGhostRoleEvent args)
    {
        try
        {
            if (!args.TookRole)
                return;

            if (_ticker.RunLevel != GameRunLevel.InRound)
                return;

            if (_ghostWatchPrototypeIds.Count == 0)
                return;

            if (!TryComp(ent.Owner, out MetaDataComponent? meta) || meta.EntityPrototype is not { } entityProto)
                return;

            if (!IsGhostWatchlisted(entityProto))
                return;

            var session = args.Player;
            var details = JsonSerializer.SerializeToDocument(new
            {
                watched_entity_prototype = entityProto.ID,
                ghost_role_name = ent.Comp.RoleName
            });

            TryEnqueue(new PendingRow
            {
                UserId = session.UserId.UserId,
                LastSeenUserName = session.Name,
                RoundId = _ticker.RoundId,
                Type = PlayerMonitoringEventType.HighValueGhostRoleTaken,
                Utc = DateTime.UtcNow,
                Details = details
            });
        }
        catch (Exception e)
        {
            _sawmill.Error($"OnGhostRoleTakenForMonitoring: {e}");
        }
    }

    private bool IsGhostWatchlisted(EntityPrototype proto)
    {
        if (_ghostWatchPrototypeIds.Contains(proto.ID))
            return true;

        foreach (var ancestorId in EnumeratePrototypeAncestors(proto))
        {
            if (_ghostWatchPrototypeIds.Contains(ancestorId))
                return true;
        }

        return false;
    }

    private IEnumerable<string> EnumeratePrototypeAncestors(EntityPrototype proto)
    {
        var queue = new Queue<string>();
        var visited = new HashSet<string>();

        foreach (var parent in proto.Parents ?? [])
        {
            queue.Enqueue(parent);
        }

        while (queue.Count > 0)
        {
            var id = queue.Dequeue();
            if (!visited.Add(id))
                continue;

            yield return id;

            if (!_prototype.TryIndex<EntityPrototype>(id, out var parent))
                continue;

            foreach (var gp in parent.Parents ?? [])
            {
                queue.Enqueue(gp);
            }
        }
    }

    private void SchedulePrune()
    {
        if (_pruneScheduledThisRound)
            return;

        _pruneScheduledThisRound = true;
        var retentionDays = _cfg.GetCVar(CCVars.MonitoringLogRetentionDays);
        var cutoff = DateTime.UtcNow.AddDays(-Math.Max(1, retentionDays));
        _ = Task.Run(async () =>
        {
            try
            {
                var n = await _db.PrunePlayerMonitoringLogsAsync(cutoff);
                if (n > 0)
                    _sawmill.Info($"Pruned {n} player monitoring log rows older than retention.");
            }
            catch (Exception e)
            {
                _sawmill.Error($"Player monitoring prune failed: {e}");
            }
        });
    }

    private void OnNoJobs(NoJobsAvailableSpawningEvent ev)
    {
        try
        {
            var user = ev.Player.UserId;
            var type = _ticker.LobbyEnabled
                ? PlayerMonitoringEventType.NoJobWaitLobbyRoundStart
                : PlayerMonitoringEventType.NoJobBecameObserver;

            var key = (user, type);
            if (!_loggedNoJob.Add(key))
                return;

            var details = JsonSerializer.SerializeToDocument(new { lobby_enabled = _ticker.LobbyEnabled });
            TryEnqueue(new PendingRow
            {
                UserId = user.UserId,
                LastSeenUserName = ev.Player.Name,
                RoundId = _ticker.RoundId,
                Type = type,
                Utc = DateTime.UtcNow,
                Details = details
            });
        }
        catch (Exception e)
        {
            _sawmill.Error($"OnNoJobs monitoring: {e}");
        }
    }

    private void OnSpawnComplete(PlayerSpawnCompleteEvent ev)
    {
        try
        {
            if (string.IsNullOrEmpty(ev.JobId))
                return;

            _spawnedJob[ev.Player.UserId] = (ev.JobId, _timing.CurTime);
        }
        catch (Exception e)
        {
            _sawmill.Error($"OnSpawnComplete monitoring: {e}");
        }
    }

    /// <summary>
    /// Called from <see cref="GameTicker"/> when the player is dumped to observer after reconnect (mind without entity / attach failed).
    /// </summary>
    public void OnReconnectDumpedToObserver(ICommonSession session, string subReason)
    {
        try
        {
            var details = JsonSerializer.SerializeToDocument(new
            {
                sub_reason = subReason,
                lobby_enabled = _ticker.LobbyEnabled
            });

            TryEnqueue(new PendingRow
            {
                UserId = session.UserId.UserId,
                LastSeenUserName = session.Name,
                RoundId = _ticker.RoundId,
                Type = PlayerMonitoringEventType.ReconnectDumpedToObserver,
                Utc = DateTime.UtcNow,
                Details = details
            });

            if (_cfg.GetCVar(CCVars.MonitoringAdminLogMirrorEnabled))
            {
                _adminLog.Add(LogType.PlayerMonitoringReconnectObserver, LogImpact.Medium,
                    $"Player monitoring: {session.Name} reconnect observer ({subReason})");
            }
        }
        catch (Exception e)
        {
            _sawmill.Error($"OnReconnectDumpedToObserver: {e}");
        }
    }

    /// <summary>
    /// Called from cryostorage when a player enters cryo.
    /// </summary>
    public void OnCryoEnter(NetUserId userId, string lastSeenUserName, EntityUid station)
    {
        try
        {
            if (_ticker.RunLevel != GameRunLevel.InRound)
                return;

            if (!_spawnedJob.TryGetValue(userId, out var snap))
                return;

            if (!_mind.TryGetMind(userId, out var mindId, out _))
                return;

            if (_roles.MindIsAntagonist(mindId))
                return;

            var minutes = (_timing.CurTime - snap.SpawnTime).TotalMinutes;
            var stationName = MetaData(station).EntityName;
            var details = JsonSerializer.SerializeToDocument(new
            {
                exit_kind = "cryo",
                job = snap.JobId,
                station = stationName,
                minutes_in_round = minutes
            });

            TryEnqueue(new PendingRow
            {
                UserId = userId.UserId,
                LastSeenUserName = lastSeenUserName,
                RoundId = _ticker.RoundId,
                Type = PlayerMonitoringEventType.MidroundExitJobNonAntag,
                Utc = DateTime.UtcNow,
                Details = details
            });

            _leftThisRound.Add(userId);

            if (_cfg.GetCVar(CCVars.MonitoringAdminLogMirrorEnabled))
            {
                _adminLog.Add(LogType.PlayerMonitoringMidroundExit, LogImpact.Medium,
                    $"Player monitoring: {lastSeenUserName} cryo (non-antag, job {snap.JobId})");
            }
        }
        catch (Exception e)
        {
            _sawmill.Error($"OnCryoEnter monitoring: {e}");
        }
    }

    private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs args)
    {
        try
        {
            if (args.NewStatus == SessionStatus.Connected)
            {
                _midroundDisconnectPending.Remove(args.Session.UserId);
                return;
            }

            if (args.NewStatus == SessionStatus.InGame)
            {
                if (_ticker.RunLevel == GameRunLevel.InRound &&
                    _leftThisRound.Remove(args.Session.UserId))
                {
                    var details = JsonSerializer.SerializeToDocument(new { round_id = _ticker.RoundId });
                    TryEnqueue(new PendingRow
                    {
                        UserId = args.Session.UserId.UserId,
                        LastSeenUserName = args.Session.Name,
                        RoundId = _ticker.RoundId,
                        Type = PlayerMonitoringEventType.SameRoundReconnect,
                        Utc = DateTime.UtcNow,
                        Details = details
                    });
                }

                return;
            }

            if (args.NewStatus != SessionStatus.Disconnected)
                return;

            TryMidroundDisconnect(args.Session, disconnectReason: null, redialFlag: null);
        }
        catch (Exception e)
        {
            _sawmill.Error($"OnPlayerStatusChanged monitoring: {e}");
        }
    }

    private void OnNetDisconnect(object? sender, NetDisconnectedArgs args)
    {
        try
        {
            if (_playerManager.TryGetSessionByChannel(args.Channel, out var session))
            {
                TryMidroundDisconnect(session, args.Reason, args.RedialFlag);
                return;
            }

            var userId = args.Channel.UserId;
            var lastName = _playerManager.TryGetPlayerData(userId, out var data)
                ? data.UserName
                : userId.UserId.ToString();

            TryMidroundDisconnectByUserId(userId, lastName, args.Reason, args.RedialFlag);
        }
        catch (Exception e)
        {
            _sawmill.Error($"OnNetDisconnect monitoring: {e}");
        }
    }

    private void TryMidroundDisconnectByUserId(NetUserId userId, string lastName, string reason, bool redialFlag)
    {
        if (_ticker.RunLevel != GameRunLevel.InRound)
            return;

        if (!_spawnedJob.TryGetValue(userId, out var snap))
        {
            TryObserverOnlyDisconnect(userId, lastName, reason, redialFlag);
            return;
        }

        if (!_mind.TryGetMind(userId, out var mindId, out _))
            return;

        if (_roles.MindIsAntagonist(mindId))
            return;

        MergeOrEnqueueMidroundDisconnect(userId, lastName, snap, reason, redialFlag);
    }

    private void TryMidroundDisconnect(ICommonSession session, string? disconnectReason, bool? redialFlag)
    {
        if (_ticker.RunLevel != GameRunLevel.InRound)
            return;

        var userId = session.UserId;
        if (!_spawnedJob.TryGetValue(userId, out var snap))
        {
            TryObserverOnlyDisconnect(userId, session.Name, disconnectReason, redialFlag);
            return;
        }

        if (!_mind.TryGetMind(session, out var mindId, out _))
            return;

        if (_roles.MindIsAntagonist(mindId))
            return;

        MergeOrEnqueueMidroundDisconnect(userId, session.Name, snap, disconnectReason, redialFlag);
    }

    /// <summary>
    /// Disconnect while ghosting / observing without ever receiving a crew spawn snapshot this round.
    /// </summary>
    private void TryObserverOnlyDisconnect(NetUserId userId, string lastSeenName, string? disconnectReason, bool? redialFlag)
    {
        if (!_mind.TryGetMind(userId, out var mindIdNullable, out var mindComp) || mindIdNullable is not { } mindId)
            return;

        if (_roles.MindHasRole<JobRoleComponent>(mindId))
            return;

        var observerLike = _roles.MindHasRole<ObserverRoleComponent>(mindId);
        if (!observerLike && mindComp.CurrentEntity is { } body && Exists(body) && !TerminatingOrDeleted(body) &&
            HasComp<GhostComponent>(body))
        {
            observerLike = true;
        }

        if (!observerLike)
            return;

        double? minsSinceRound = null;
        if (_roundInRoundSince is { } started)
            minsSinceRound = (_timing.CurTime - started).TotalMinutes;

        var details = JsonSerializer.SerializeToDocument(new
        {
            exit_kind = "observer_only",
            disconnect_reason = disconnectReason,
            redial_flag = redialFlag,
            minutes_since_round_start = minsSinceRound
        });

        TryEnqueue(new PendingRow
        {
            UserId = userId.UserId,
            LastSeenUserName = lastSeenName,
            RoundId = _ticker.RoundId,
            Type = PlayerMonitoringEventType.ObserverOnlyMidroundDisconnect,
            Utc = DateTime.UtcNow,
            Details = details
        });
    }

    private void MergeOrEnqueueMidroundDisconnect(NetUserId userId, string lastSeenName, (string JobId, TimeSpan SpawnTime) snap, string? disconnectReason, bool? redialFlag)
    {
        var minutes = (_timing.CurTime - snap.SpawnTime).TotalMinutes;

        if (_midroundDisconnectPending.TryGetValue(userId, out var pending))
        {
            var patch = JsonSerializer.SerializeToDocument(new
            {
                disconnect_reason = disconnectReason,
                redial_flag = redialFlag
            });
            var merged = ServerDbBase.MergeMonitoringDetails(pending.Details, patch);
            pending.Details?.Dispose();
            patch.Dispose();
            pending.Details = merged;
            return;
        }

        var details = JsonSerializer.SerializeToDocument(new
        {
            exit_kind = "disconnect",
            job = snap.JobId,
            minutes_in_round = minutes,
            disconnect_reason = disconnectReason,
            redial_flag = redialFlag
        });

        var row = new PendingRow
        {
            UserId = userId.UserId,
            LastSeenUserName = lastSeenName,
            RoundId = _ticker.RoundId,
            Type = PlayerMonitoringEventType.MidroundExitJobNonAntag,
            Utc = DateTime.UtcNow,
            Details = details
        };

        _midroundDisconnectPending[userId] = row;
        TryEnqueue(row);
        _leftThisRound.Add(userId);

        if (_cfg.GetCVar(CCVars.MonitoringAdminLogMirrorEnabled))
        {
            _adminLog.Add(LogType.PlayerMonitoringMidroundExit, LogImpact.Medium,
                $"Player monitoring: {lastSeenName} disconnect (non-antag, job {snap.JobId})");
        }
    }

    private void TryEnqueue(PendingRow row)
    {
        var cap = _cfg.GetCVar(CCVars.MonitoringLogQueueMax);
        lock (_queueLock)
        {
            if (_queue.Count >= cap)
            {
                _drops++;
                row.Details?.Dispose();
                if (_drops % 100 == 1)
                    _sawmill.Warning($"Player monitoring queue full (drops: {_drops})");
                return;
            }

            _queue.Enqueue(row);
        }
    }

    private sealed class PendingRow
    {
        public required Guid UserId { get; init; }
        public required string LastSeenUserName { get; init; }
        public required int RoundId { get; init; }
        public required PlayerMonitoringEventType Type { get; init; }
        public required DateTime Utc { get; init; }
        public JsonDocument? Details { get; set; }
    }
}
