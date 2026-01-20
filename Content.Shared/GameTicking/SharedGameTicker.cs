// SPDX-FileCopyrightText: 2025 Matthew Herber
// SPDX-FileCopyrightText: 2025 Nemanja
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2025 pathetic meowmeow
// SPDX-FileCopyrightText: 2025 SpaceManiac
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Fildrance
// SPDX-FileCopyrightText: 2024 Vasilis
// SPDX-FileCopyrightText: 2024 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2024 DoutorWhite
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Jessica M
// SPDX-FileCopyrightText: 2022 Morber
// SPDX-FileCopyrightText: 2022 KIBORG04
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 ShadowCommander
// SPDX-FileCopyrightText: 2022 Jesse Rougeau
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 AJCM-git
// SPDX-FileCopyrightText: 2020 scuffedjays
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-License-Identifier: MIT

using Content.Shared.Roles;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Replays;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Timing;
using Robust.Shared.Audio;
using Content.Shared.GameTicking.Prototypes;

namespace Content.Shared.GameTicking
{
    public abstract class SharedGameTicker : EntitySystem
    {
        [Dependency] private readonly IReplayRecordingManager _replay = default!;
        [Dependency] private readonly IGameTiming _gameTiming = default!;

        /// <summary>
        ///     A list storing the start times of all game rules that have been started this round.
        ///     Game rules can be started and stopped at any time, including midround.
        /// </summary>
        public abstract IReadOnlyList<(TimeSpan, string)> AllPreviousGameRules { get; }

        // See ideally these would be pulled from the job definition or something.
        // But this is easier, and at least it isn't hardcoded.
        //TODO: Move these, they really belong in StationJobsSystem or a cvar.
        public static readonly ProtoId<JobPrototype> FallbackOverflowJob = "Passenger";

        public const string FallbackOverflowJobName = "job-name-passenger";

        // TODO network.
        // Probably most useful for replays, round end info, and probably things like lobby menus.
        [ViewVariables]
        public int RoundId { get; protected set; }
        [ViewVariables] public TimeSpan RoundStartTimeSpan { get; protected set; }

        public override void Initialize()
        {
            base.Initialize();
            _replay.RecordingStarted += OnRecordingStart;
        }

        public override void Shutdown()
        {
            _replay.RecordingStarted -= OnRecordingStart;
        }

        private void OnRecordingStart(MappingDataNode metadata, List<object> events)
        {
            if (RoundId != 0)
            {
                metadata["roundId"] = new ValueDataNode(RoundId.ToString());
            }
        }

        public TimeSpan RoundDuration()
        {
            return _gameTiming.CurTime.Subtract(RoundStartTimeSpan);
        }
    }

    [Serializable, NetSerializable]
    public sealed class TickerJoinLobbyEvent : EntityEventArgs
    {
    }

    [Serializable, NetSerializable]
    public sealed class TickerJoinGameEvent : EntityEventArgs
    {
    }

    [Serializable, NetSerializable]
    public sealed class TickerLateJoinStatusEvent : EntityEventArgs
    {
        // TODO: Make this a replicated CVar, honestly.
        public bool Disallowed { get; }

        public TickerLateJoinStatusEvent(bool disallowed)
        {
            Disallowed = disallowed;
        }
    }

    [Serializable, NetSerializable]
    public sealed class TickerConnectionStatusEvent : EntityEventArgs
    {
        public TimeSpan RoundStartTimeSpan { get; }
        public TickerConnectionStatusEvent(TimeSpan roundStartTimeSpan)
        {
            RoundStartTimeSpan = roundStartTimeSpan;
        }
    }

    [Serializable, NetSerializable]
    public sealed class TickerLobbyStatusEvent : EntityEventArgs
    {
        public bool IsRoundStarted { get; }
        public ProtoId<LobbyBackgroundPrototype>? LobbyBackground { get; }
        public bool YouAreReady { get; }
        // UTC.
        public TimeSpan StartTime { get; }
        public TimeSpan RoundStartTimeSpan { get; }
        public bool Paused { get; }

        public TickerLobbyStatusEvent(bool isRoundStarted, ProtoId<LobbyBackgroundPrototype>? lobbyBackground, bool youAreReady, TimeSpan startTime, TimeSpan preloadTime, TimeSpan roundStartTimeSpan, bool paused)
        {
            IsRoundStarted = isRoundStarted;
            LobbyBackground = lobbyBackground;
            YouAreReady = youAreReady;
            StartTime = startTime;
            RoundStartTimeSpan = roundStartTimeSpan;
            Paused = paused;
        }
    }

    [Serializable, NetSerializable]
    public sealed class TickerLobbyInfoEvent : EntityEventArgs
    {
        public string TextBlob { get; }

        public TickerLobbyInfoEvent(string textBlob)
        {
            TextBlob = textBlob;
        }
    }

    [Serializable, NetSerializable]
    public sealed class TickerLobbyCountdownEvent : EntityEventArgs
    {
        /// <summary>
        /// The game time that the game will start at.
        /// </summary>
        public TimeSpan StartTime { get; }

        /// <summary>
        /// Whether or not the countdown is paused
        /// </summary>
        public bool Paused { get; }

        public TickerLobbyCountdownEvent(TimeSpan startTime, bool paused)
        {
            StartTime = startTime;
            Paused = paused;
        }
    }

    [Serializable, NetSerializable]
    public sealed class TickerJobsAvailableEvent(
        Dictionary<NetEntity, string> stationNames,
        Dictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> jobsAvailableByStation)
        : EntityEventArgs
    {
        /// <summary>
        /// The Status of the Player in the lobby (ready, observer, ...)
        /// </summary>
        public Dictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> JobsAvailableByStation { get; } = jobsAvailableByStation;

        public Dictionary<NetEntity, string> StationNames { get; } = stationNames;
    }

    [Serializable, NetSerializable, DataDefinition]
    public sealed partial class RoundEndMessageEvent : EntityEventArgs
    {
        [Serializable, NetSerializable, DataDefinition]
        public partial struct RoundEndPlayerInfo
        {
            [DataField]
            public string PlayerOOCName;

            [DataField]
            public string? PlayerICName;

            [DataField, NonSerialized]
            public NetUserId? PlayerGuid;

            public string Role;

            [DataField, NonSerialized]
            public string[] JobPrototypes;

            [DataField, NonSerialized]
            public string[] AntagPrototypes;

            public NetEntity? PlayerNetEntity;

            [DataField]
            public bool Antag;

            [DataField]
            public bool Observer;

            public bool Connected;
        }

        public string GamemodeTitle { get; }
        public string RoundEndText { get; }
        public TimeSpan RoundDuration { get; }
        public int RoundId { get; }
        public int PlayerCount { get; }
        public RoundEndPlayerInfo[] AllPlayersEndInfo { get; }

        /// <summary>
        /// Sound gets networked due to how entity lifecycle works between client / server and to avoid clipping.
        /// </summary>
        public ResolvedSoundSpecifier? RestartSound;

        public RoundEndMessageEvent(
            string gamemodeTitle,
            string roundEndText,
            TimeSpan roundDuration,
            int roundId,
            int playerCount,
            RoundEndPlayerInfo[] allPlayersEndInfo,
            ResolvedSoundSpecifier? restartSound)
        {
            GamemodeTitle = gamemodeTitle;
            RoundEndText = roundEndText;
            RoundDuration = roundDuration;
            RoundId = roundId;
            PlayerCount = playerCount;
            AllPlayersEndInfo = allPlayersEndInfo;
            RestartSound = restartSound;
        }
    }

    [Serializable, NetSerializable]
    public enum PlayerGameStatus : sbyte
    {
        NotReadyToPlay = 0,
        ReadyToPlay,
        JoinedGame,
    }
}
