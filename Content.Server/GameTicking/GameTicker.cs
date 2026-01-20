// SPDX-FileCopyrightText: 2024 Fildrance
// SPDX-FileCopyrightText: 2025 Fildrance
// SPDX-FileCopyrightText: 2025 beck-thompson
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 ElectroJr
// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-FileCopyrightText: 2024 Mervill
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 metalgearsloth
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2020 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2023 Riggle
// SPDX-FileCopyrightText: 2023 Jezithyr
// SPDX-FileCopyrightText: 2022 Morber
// SPDX-FileCopyrightText: 2022 Veritius
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Jesse Rougeau
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2022 ShadowCommander
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2020 ike709
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 20kdc
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DTanxxx
// SPDX-FileCopyrightText: 2020 creadth
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-FileCopyrightText: 2020 scuffedjays
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-FileCopyrightText: 2020 DamianX
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.Database;
using Content.Server.Ghost;
using Content.Server.Maps;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Preferences.Managers;
using Content.Server.ServerUpdates;
using Content.Server.Station.Systems;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.GameTicking;
using Content.Shared.Mind;
using Content.Shared.Roles;
using Robust.Server;
using Robust.Server.GameObjects;
using Robust.Server.GameStates;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Console;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
#if EXCEPTION_TOLERANCE
using Robust.Shared.Exceptions;
#endif

namespace Content.Server.GameTicking
{
    public sealed partial class GameTicker : SharedGameTicker
    {
        [Dependency] private readonly IAdminLogManager _adminLogger = default!;
        [Dependency] private readonly IBanManager _banManager = default!;
        [Dependency] private readonly IBaseServer _baseServer = default!;
        [Dependency] private readonly IChatManager _chatManager = default!;
        [Dependency] private readonly IConsoleHost _consoleHost = default!;
        [Dependency] private readonly IGameMapManager _gameMapManager = default!;
        [Dependency] private readonly IGameTiming _gameTiming = default!;
        [Dependency] private readonly ILogManager _logManager = default!;
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly IRobustRandom _robustRandom = default!;
#if EXCEPTION_TOLERANCE
        [Dependency] private readonly IRuntimeLog _runtimeLog = default!;
#endif
        [Dependency] private readonly IServerPreferencesManager _prefsManager = default!;
        [Dependency] private readonly IServerDbManager _db = default!;
        [Dependency] private readonly ChatSystem _chatSystem = default!;
        [Dependency] private readonly MapLoaderSystem _loader = default!;
        [Dependency] private readonly SharedMapSystem _map = default!;
        [Dependency] private readonly GhostSystem _ghost = default!;
        [Dependency] private readonly SharedMindSystem _mind = default!;
        [Dependency] private readonly PlayTimeTrackingSystem _playTimeTrackings = default!;
        [Dependency] private readonly PvsOverrideSystem _pvsOverride = default!;
        [Dependency] private readonly ServerUpdateManager _serverUpdates = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;
        [Dependency] private readonly StationJobsSystem _stationJobs = default!;
        [Dependency] private readonly StationSpawningSystem _stationSpawning = default!;
        [Dependency] private readonly SharedTransformSystem _transform = default!;
        [Dependency] private readonly UserDbDataManager _userDb = default!;
        [Dependency] private readonly MetaDataSystem _metaData = default!;
        [Dependency] private readonly SharedRoleSystem _roles = default!;
        [Dependency] private readonly ServerDbEntryManager _dbEntryManager = default!;

        [ViewVariables] private bool _initialized;
        [ViewVariables] private bool _postInitialized;

        [ViewVariables] public MapId DefaultMap { get; private set; }

        private ISawmill _sawmill = default!;

        private bool _randomizeCharacters;

        public override void Initialize()
        {
            base.Initialize();

            DebugTools.Assert(!_initialized);
            DebugTools.Assert(!_postInitialized);

            _sawmill = _logManager.GetSawmill("ticker");
            _sawmillReplays = _logManager.GetSawmill("ticker.replays");

            Subs.CVar(_cfg, CCVars.ICRandomCharacters, e => _randomizeCharacters = e, true);

            // Initialize the other parts of the game ticker.
            InitializeStatusShell();
            InitializeCVars();
            InitializePlayer();
            InitializeLobbyBackground();
            InitializeGamePreset();
            DebugTools.Assert(_prototypeManager.Index(FallbackOverflowJob).Name == FallbackOverflowJobName,
                "Overflow role does not have the correct name!");
            InitializeGameRules();
            InitializeReplays();
            _initialized = true;
        }

        public void PostInitialize()
        {
            DebugTools.Assert(_initialized);
            DebugTools.Assert(!_postInitialized);

            // We restart the round now that entities are initialized and prototypes have been loaded.
            if (!DummyTicker)
                RestartRound();

            _postInitialized = true;
        }

        public override void Shutdown()
        {
            base.Shutdown();

            ShutdownGameRules();
        }

        private void SendServerMessage(string message)
        {
            var wrappedMessage = Loc.GetString("chat-manager-server-wrap-message", ("message", message));
            _chatManager.ChatMessageToAll(ChatChannel.Server, message, wrappedMessage, default, false, true);
        }

        public override void Update(float frameTime)
        {
            if (DummyTicker)
                return;
            base.Update(frameTime);
            UpdateRoundFlow(frameTime);
            UpdateGameRules();
        }
    }
}
