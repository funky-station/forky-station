// SPDX-FileCopyrightText: 2025 Matthew Herber
// SPDX-FileCopyrightText: 2025 Nemanja
// SPDX-FileCopyrightText: 2025 pathetic meowmeow
// SPDX-FileCopyrightText: 2025 SpaceManiac
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 wafehling
// SPDX-FileCopyrightText: 2024 Fildrance
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2024 DoutorWhite
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Jessica M
// SPDX-FileCopyrightText: 2022 KIBORG04
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2019 ShadowCommander
// SPDX-FileCopyrightText: 2020 ShadowCommander
// SPDX-FileCopyrightText: 2022 ShadowCommander
// SPDX-FileCopyrightText: 2022 Jesse Rougeau
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2019 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 scuffedjays
// SPDX-FileCopyrightText: 2020 DamianX
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-License-Identifier: MIT

using Content.Client.Administration.Managers;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Client.RoundEnd;
using Content.Shared.GameTicking;
using Content.Shared.GameWindow;
using Content.Shared.Roles;
using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;
using Content.Shared.GameTicking.Prototypes;

namespace Content.Client.GameTicking.Managers
{
    [UsedImplicitly]
    public sealed class ClientGameTicker : SharedGameTicker
    {
        [Dependency] private readonly IStateManager _stateManager = default!;
        [Dependency] private readonly IClientAdminManager _admin = default!;
        [Dependency] private readonly IClyde _clyde = default!;
        [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;

        private Dictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>>  _jobsAvailable = new();
        private Dictionary<NetEntity, string> _stationNames = new();

        [ViewVariables] public bool AreWeReady { get; private set; }
        [ViewVariables] public bool IsGameStarted { get; private set; }
        [ViewVariables] public ResolvedSoundSpecifier? RestartSound { get; private set; }
        [ViewVariables] public ProtoId<LobbyBackgroundPrototype>? LobbyBackground { get; private set; }
        [ViewVariables] public bool DisallowedLateJoin { get; private set; }
        [ViewVariables] public string? ServerInfoBlob { get; private set; }
        [ViewVariables] public TimeSpan StartTime { get; private set; }
        [ViewVariables] public new bool Paused { get; private set; }

        public override IReadOnlyList<(TimeSpan, string)> AllPreviousGameRules => new List<(TimeSpan, string)>();

        [ViewVariables] public IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> JobsAvailable => _jobsAvailable;
        [ViewVariables] public IReadOnlyDictionary<NetEntity, string> StationNames => _stationNames;

        public event Action? InfoBlobUpdated;
        public event Action? LobbyStatusUpdated;
        public event Action? LobbyLateJoinStatusUpdated;
        public event Action<IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>>>? LobbyJobsAvailableUpdated;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeNetworkEvent<TickerJoinLobbyEvent>(JoinLobby);
            SubscribeNetworkEvent<TickerJoinGameEvent>(JoinGame);
            SubscribeNetworkEvent<TickerConnectionStatusEvent>(ConnectionStatus);
            SubscribeNetworkEvent<TickerLobbyStatusEvent>(LobbyStatus);
            SubscribeNetworkEvent<TickerLobbyInfoEvent>(LobbyInfo);
            SubscribeNetworkEvent<TickerLobbyCountdownEvent>(LobbyCountdown);
            SubscribeNetworkEvent<RoundEndMessageEvent>(RoundEnd);
            SubscribeNetworkEvent<RequestWindowAttentionEvent>(OnAttentionRequest);
            SubscribeNetworkEvent<TickerLateJoinStatusEvent>(LateJoinStatus);
            SubscribeNetworkEvent<TickerJobsAvailableEvent>(UpdateJobsAvailable);

            _admin.AdminStatusUpdated += OnAdminUpdated;
            OnAdminUpdated();
        }

        public override void Shutdown()
        {
            _admin.AdminStatusUpdated -= OnAdminUpdated;
            base.Shutdown();
        }

        private void OnAdminUpdated()
        {
            // Hide some map/grid related logs from clients. This is to try prevent some easy metagaming by just
            // reading the console. E.g., logs like this one could leak the nuke station/grid:
            // > Grid NT-Arrivals 1101 (122/n25896) changed parent. Old parent: map 10 (121/n25895). New parent: FTL (123/n26470)
#if !DEBUG
            EntityManager.System<SharedMapSystem>().Log.Level = _admin.IsAdmin() ? LogLevel.Info : LogLevel.Warning;
#endif
        }

        private void OnAttentionRequest(RequestWindowAttentionEvent ev)
        {
            _clyde.RequestWindowAttention();
        }

        private void LateJoinStatus(TickerLateJoinStatusEvent message)
        {
            DisallowedLateJoin = message.Disallowed;
            LobbyLateJoinStatusUpdated?.Invoke();
        }

        private void UpdateJobsAvailable(TickerJobsAvailableEvent message)
        {
            _jobsAvailable.Clear();

            foreach (var (job, data) in message.JobsAvailableByStation)
            {
                _jobsAvailable[job] = data;
            }

            _stationNames.Clear();
            foreach (var weh in message.StationNames)
            {
                _stationNames[weh.Key] = weh.Value;
            }

            LobbyJobsAvailableUpdated?.Invoke(JobsAvailable);
        }

        private void JoinLobby(TickerJoinLobbyEvent message)
        {
            _stateManager.RequestStateChange<LobbyState>();
        }

        private void ConnectionStatus(TickerConnectionStatusEvent message)
        {
            RoundStartTimeSpan = message.RoundStartTimeSpan;
        }

        private void LobbyStatus(TickerLobbyStatusEvent message)
        {
            StartTime = message.StartTime;
            RoundStartTimeSpan = message.RoundStartTimeSpan;
            IsGameStarted = message.IsRoundStarted;
            AreWeReady = message.YouAreReady;
            LobbyBackground = message.LobbyBackground;
            Paused = message.Paused;

            LobbyStatusUpdated?.Invoke();
        }

        private void LobbyInfo(TickerLobbyInfoEvent message)
        {
            ServerInfoBlob = message.TextBlob;

            InfoBlobUpdated?.Invoke();
        }

        private void JoinGame(TickerJoinGameEvent message)
        {
            _stateManager.RequestStateChange<GameplayState>();
        }

        private void LobbyCountdown(TickerLobbyCountdownEvent message)
        {
            StartTime = message.StartTime;
            Paused = message.Paused;
        }

        private void RoundEnd(RoundEndMessageEvent message)
        {
            // Force an update in the event of this song being the same as the last.
            RestartSound = message.RestartSound;

            _userInterfaceManager.GetUIController<RoundEndSummaryUIController>().OpenRoundEndSummaryWindow(message);
        }
    }
}
