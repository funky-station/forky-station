using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Events;
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
using Robust.Shared.Asynchronous;
using Robust.Shared.Timing;

namespace Content.Server.Administration.Logs;

/// <summary>
/// Emits player-monitoring rows as <see cref="LogType.PlayerMonitoring"/> admin logs; subscribes to game/session events. Observation-only.
/// </summary>
public sealed class PlayerMonitoringLogSystem : EntitySystem
{
    private const int MonitoringEmitQueueMax = 4096;

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
    [Dependency] private readonly ITaskManager _taskManager = default!;

    private ISawmill _sawmill = default!;

    private readonly Queue<PendingRow> _queue = new();
    private readonly object _queueLock = new();

    private readonly Dictionary<NetUserId, (string JobId, TimeSpan SpawnTime)> _spawnedJob = new();

    private readonly HashSet<(NetUserId User, PlayerMonitoringEventType Type)> _loggedNoJob = new();

    private readonly HashSet<NetUserId> _leftThisRound = new();

    private readonly Dictionary<NetUserId, PendingRow> _midroundDisconnectPending = new();

    private TimeSpan? _roundInRoundSince;

    private int _drops;
    private volatile int _flushGate;
    private TimeSpan _nextFlush;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("admin.player_monitoring");

        SubscribeLocalEvent<NoJobsAvailableSpawningEvent>(OnNoJobs);
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnSpawnComplete);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundCleanup);
        SubscribeLocalEvent<GameRunLevelChangedEvent>(OnGameRunLevelChanged);

        _playerManager.PlayerStatusChanged += OnPlayerStatusChanged;
        _net.Disconnect += OnNetDisconnect;

        _nextFlush = _timing.CurTime;
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _playerManager.PlayerStatusChanged -= OnPlayerStatusChanged;
        _net.Disconnect -= OnNetDisconnect;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_timing.CurTime < _nextFlush)
            return;

        _nextFlush = _timing.CurTime + TimeSpan.FromMilliseconds(500);
        FlushEmitQueueMainThread();
    }

    private static JsonDocument BuildMergedMonitoringJson(PlayerMonitoringEventType kind, JsonDocument? details)
    {
        using var ms = new MemoryStream();
        using (var writer = new Utf8JsonWriter(ms))
        {
            writer.WriteStartObject();
            writer.WriteNumber("kind", (int)kind);
            if (details != null)
            {
                foreach (var p in details.RootElement.EnumerateObject())
                {
                    writer.WritePropertyName(p.Name);
                    p.Value.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        return JsonDocument.Parse(ms.ToArray());
    }

    private void FlushEmitQueueMainThread()
    {
        if (Interlocked.Exchange(ref _flushGate, 1) != 0)
            return;

        try
        {
            List<PendingRow>? batch;
            lock (_queueLock)
            {
                if (_queue.Count == 0)
                    batch = null;
                else
                {
                    var maxBatch = 64;
                    batch = new List<PendingRow>(Math.Min(maxBatch, _queue.Count));
                    while (_queue.Count > 0 && batch.Count < maxBatch)
                        batch.Add(_queue.Dequeue());
                }
            }

            if (batch == null || batch.Count == 0)
                return;

            foreach (var row in batch)
            {
                try
                {
                    using var merged = BuildMergedMonitoringJson(row.Type, row.Details);
                    var payload = merged.RootElement.GetRawText();
                    _adminLog.Add(LogType.PlayerMonitoring, LogImpact.Low,
                        $"{new AdminLogAttributedUser(new NetUserId(row.UserId))} player monitoring {payload}");
                    row.Details?.Dispose();
                    if (row.Type == PlayerMonitoringEventType.MidroundExitJobNonAntag)
                        _midroundDisconnectPending.Remove(new NetUserId(row.UserId));
                }
                catch (Exception e)
                {
                    _sawmill.Error($"Error emitting player monitoring admin log: {e}");
                    row.Details?.Dispose();
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
        var endingRoundId = _ticker.RoundId;

        _spawnedJob.Clear();
        _loggedNoJob.Clear();
        _leftThisRound.Clear();
        _midroundDisconnectPending.Clear();
        _roundInRoundSince = null;

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

            var payloads = new List<(Guid UserId, string Payload)>();
            foreach (var entry in flagged)
            {
                using var details = JsonSerializer.SerializeToDocument(new
                {
                    max_idle_minutes = entry.MaxIdleMinutes,
                    threshold_minutes = (double)threshold,
                    analysis_end_utc = roundEndUtc
                });
                using var merged = BuildMergedMonitoringJson(PlayerMonitoringEventType.LongAfkFromAdminLogsRoundEnd, details);
                payloads.Add((entry.UserId, merged.RootElement.GetRawText()));
            }

            _taskManager.RunOnMainThread(() =>
            {
                foreach (var (userId, payload) in payloads)
                {
                    _adminLog.Add(LogType.PlayerMonitoring, LogImpact.Low,
                        $"{new AdminLogAttributedUser(new NetUserId(userId))} player monitoring {payload}");
                }
            });
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

    private void OnNoJobs(NoJobsAvailableSpawningEvent ev)
    {
        try
        {
            var user = ev.Player.UserId;
            PlayerMonitoringEventType type;
            if (_ticker.LobbyEnabled)
            {
                type = ev.LateJoin
                    ? PlayerMonitoringEventType.NoJobWaitLobbyLateJoin
                    : PlayerMonitoringEventType.NoJobWaitLobbyRoundStart;
            }
            else
            {
                type = PlayerMonitoringEventType.NoJobBecameObserver;
            }

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

        double? minsSinceRoundStart = null;
        if (_roundInRoundSince is { } rs)
            minsSinceRoundStart = (_timing.CurTime - rs).TotalMinutes;

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
            minutes_since_round_start = minsSinceRoundStart,
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
                $"{new AdminLogAttributedUser(userId)} Player monitoring: {lastSeenName} disconnect (non-antag, job {snap.JobId})");
        }
    }

    private void TryEnqueue(PendingRow row)
    {
        lock (_queueLock)
        {
            if (_queue.Count >= MonitoringEmitQueueMax)
            {
                _drops++;
                row.Details?.Dispose();
                if (_drops % 100 == 1)
                    _sawmill.Warning($"Player monitoring emit queue full (drops: {_drops})");
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
