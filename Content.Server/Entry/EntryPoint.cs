// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2025 Leon Friedrich
// SPDX-FileCopyrightText: 2025 Fildrance
// SPDX-FileCopyrightText: 2025 beck-thompson
// SPDX-FileCopyrightText: 2024 Simon
// SPDX-FileCopyrightText: 2025 Simon
// SPDX-FileCopyrightText: 2025 B_Kirill
// SPDX-FileCopyrightText: 2017 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Palladinium
// SPDX-FileCopyrightText: 2025 Myra
// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2023 Moony
// SPDX-FileCopyrightText: 2023 Riggle
// SPDX-FileCopyrightText: 2019 metalgearsloth
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Ygg01
// SPDX-FileCopyrightText: 2022 Francesco
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 20kdc
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2019 ShadowCommander
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2022 ShadowCommander
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Sam Weaver
// SPDX-FileCopyrightText: 2018 Acruid
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2021 moonheart08
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2020 Tyler Young
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-FileCopyrightText: 2020 DamianX
// SPDX-FileCopyrightText: 2020 Swept
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-FileCopyrightText: 2020 Jackson Lewis
// SPDX-FileCopyrightText: 2019 moneyl
// SPDX-FileCopyrightText: 2020 moneyl
// SPDX-FileCopyrightText: 2019 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2019 Injazz
// SPDX-FileCopyrightText: 2019 PrPleGoo
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2019 ScumbagDog
// SPDX-FileCopyrightText: 2018 clusterfack
// SPDX-FileCopyrightText: 2017 PJB3005
// SPDX-FileCopyrightText: 2018 PJB3005
// SPDX-FileCopyrightText: 2018 Centronias
// SPDX-License-Identifier: MIT

using Content.Server.Acz;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Afk;
using Content.Server.Chat.Managers;
using Content.Server.Connection;
using Content.Server.Database;
using Content.Server.Discord.DiscordLink;
using Content.Server.EUI;
using Content.Server.GameTicking;
using Content.Server.GhostKick;
using Content.Server.GuideGenerator;
using Content.Server.Info;
using Content.Server.IoC;
using Content.Server.Maps;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.Players.JobWhitelist;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Players.RateLimiting;
using Content.Server.Preferences.Managers;
using Content.Server.ServerInfo;
using Content.Server.ServerUpdates;
using Content.Server.Voting.Managers;
using Content.Shared.CCVar;
using Content.Shared.Kitchen;
using Content.Shared.Localizations;
using Robust.Server;
using Robust.Server.ServerStatus;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Entry
{
    public sealed class EntryPoint : GameServer
    {
        internal const string ConfigPresetsDir = "/ConfigPresets/";
        private const string ConfigPresetsDirBuild = $"{ConfigPresetsDir}Build/";

        [Dependency] private readonly CVarControlManager _cvarCtrl = default!;
        [Dependency] private readonly ContentLocalizationManager _loc = default!;
        [Dependency] private readonly ContentNetworkResourceManager _netResMan = default!;
        [Dependency] private readonly DiscordChatLink _discordChatLink = default!;
        [Dependency] private readonly DiscordLink _discordLink = default!;
        [Dependency] private readonly EuiManager _euiManager = default!;
        [Dependency] private readonly GhostKickManager _ghostKick = default!;
        [Dependency] private readonly IAdminManager _admin = default!;
        [Dependency] private readonly IAdminLogManager _adminLog = default!;
        [Dependency] private readonly IAfkManager _afk = default!;
        [Dependency] private readonly IBanManager _ban = default!;
        [Dependency] private readonly IChatManager _chatSan = default!;
        [Dependency] private readonly IChatSanitizationManager _chat = default!;
        [Dependency] private readonly IComponentFactory _factory = default!;
        [Dependency] private readonly IConfigurationManager _cfg = default!;
        [Dependency] private readonly IConnectionManager _connection = default!;
        [Dependency] private readonly IEntitySystemManager _entSys = default!;
        [Dependency] private readonly IGameMapManager _gameMap = default!;
        [Dependency] private readonly ILogManager _log = default!;
        [Dependency] private readonly INodeGroupFactory _nodeFactory = default!;
        [Dependency] private readonly IPrototypeManager _proto = default!;
        [Dependency] private readonly IResourceManager _res = default!;
        [Dependency] private readonly IServerDbManager _dbManager = default!;
        [Dependency] private readonly IServerPreferencesManager _preferences = default!;
        [Dependency] private readonly IStatusHost _host = default!;
        [Dependency] private readonly IVoteManager _voteManager = default!;
        [Dependency] private readonly IWatchlistWebhookManager _watchlistWebhookManager = default!;
        [Dependency] private readonly JobWhitelistManager _job = default!;
        [Dependency] private readonly MultiServerKickManager _multiServerKick = default!;
        [Dependency] private readonly PlayTimeTrackingManager _playTimeTracking = default!;
        [Dependency] private readonly PlayerRateLimitManager _rateLimit = default!;
        [Dependency] private readonly RecipeManager _recipe = default!;
        [Dependency] private readonly RulesManager _rules = default!;
        [Dependency] private readonly ServerApi _serverApi = default!;
        [Dependency] private readonly ServerInfoManager _serverInfo = default!;
        [Dependency] private readonly ServerUpdateManager _updateManager = default!;

        public override void PreInit()
        {
            ServerContentIoC.Register(Dependencies);
            foreach (var callback in TestingCallbacks)
            {
                var cast = (ServerModuleTestingCallbacks)callback;
                cast.ServerBeforeIoC?.Invoke();
            }
        }

        /// <inheritdoc />
        public override void Init()
        {
            base.Init();
            Dependencies.BuildGraph();
            Dependencies.InjectDependencies(this);

            LoadConfigPresets(_cfg, _res, _log.GetSawmill("configpreset"));

            var aczProvider = new ContentMagicAczProvider(Dependencies);
            _host.SetMagicAczProvider(aczProvider);

            _factory.DoAutoRegistrations();
            _factory.IgnoreMissingComponents("Visuals");
            _factory.RegisterIgnore(IgnoredComponents.List);
            _factory.GenerateNetIds();

            _proto.RegisterIgnore("parallax");

            _loc.Initialize();

            var dest = _cfg.GetCVar(CCVars.DestinationFile);
            if (!string.IsNullOrEmpty(dest))
                return; //hacky but it keeps load times for the generator down.

            _log.GetSawmill("Storage").Level = LogLevel.Info;
            _log.GetSawmill("db.ef").Level = LogLevel.Info;

            _adminLog.Initialize();
            _connection.Initialize();
            _dbManager.Init();
            _preferences.Init();
            _nodeFactory.Initialize();
            _netResMan.Initialize();
            _ghostKick.Initialize();
            _serverInfo.Initialize();
            _serverApi.Initialize();
            _voteManager.Initialize();
            _updateManager.Initialize();
            _playTimeTracking.Initialize();
            _watchlistWebhookManager.Initialize();
            _job.Initialize();
            _rateLimit.Initialize();
        }

        public override void PostInit()
        {
            base.PostInit();

            _chatSan.Initialize();
            _chat.Initialize();
            var dest = _cfg.GetCVar(CCVars.DestinationFile);
            if (!string.IsNullOrEmpty(dest))
            {
                var resPath = new ResPath(dest).ToRootedPath();
                var file = _res.UserData.OpenWriteText(resPath.WithName("chem_" + dest));
                ChemistryJsonGenerator.PublishJson(file);
                file.Flush();
                file = _res.UserData.OpenWriteText(resPath.WithName("react_" + dest));
                ReactionJsonGenerator.PublishJson(file);
                file.Flush();
                Dependencies.Resolve<IBaseServer>().Shutdown("Data generation done");
                return;
            }

            _recipe.Initialize();
            _admin.Initialize();
            _afk.Initialize();
            _rules.Initialize();
            _discordLink.Initialize();
            _discordChatLink.Initialize();
            _euiManager.Initialize();
            _gameMap.Initialize();
            _entSys.GetEntitySystem<GameTicker>().PostInitialize();
            _ban.Initialize();
            _connection.PostInit();
            _multiServerKick.Initialize();
            _cvarCtrl.Initialize();
        }

        public override void Update(ModUpdateLevel level, FrameEventArgs frameEventArgs)
        {
            base.Update(level, frameEventArgs);

            switch (level)
            {
                case ModUpdateLevel.PostEngine:
                {
                    _euiManager.SendUpdates();
                    _voteManager.Update();
                    break;
                }

                case ModUpdateLevel.FramePostEngine:
                    _updateManager.Update();
                    _playTimeTracking.Update();
                    _watchlistWebhookManager.Update();
                    _connection.Update();
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            var dest = _cfg.GetCVar(CCVars.DestinationFile);
            if (!string.IsNullOrEmpty(dest))
            {
                _playTimeTracking.Shutdown();
                _dbManager.Shutdown();
            }

            _serverApi.Shutdown();

            // TODO Should this be awaited?
            _discordLink.Shutdown();
            _discordChatLink.Shutdown();
        }

        private static void LoadConfigPresets(IConfigurationManager cfg, IResourceManager res, ISawmill sawmill)
        {
            LoadBuildConfigPresets(cfg, res, sawmill);

            var presets = cfg.GetCVar(CCVars.ConfigPresets);
            if (presets == "")
                return;

            foreach (var preset in presets.Split(','))
            {
                var path = $"{ConfigPresetsDir}{preset}.toml";
                if (!res.TryContentFileRead(path, out var file))
                {
                    sawmill.Error("Unable to load config preset {Preset}!", path);
                    continue;
                }

                cfg.LoadDefaultsFromTomlStream(file);
                sawmill.Info("Loaded config preset: {Preset}", path);
            }
        }

        private static void LoadBuildConfigPresets(IConfigurationManager cfg, IResourceManager res, ISawmill sawmill)
        {
#if TOOLS
            Load(CCVars.ConfigPresetDevelopment, "development");
#endif
#if DEBUG
            Load(CCVars.ConfigPresetDebug, "debug");
#endif

#pragma warning disable CS8321
            void Load(CVarDef<bool> cVar, string name)
            {
                var path = $"{ConfigPresetsDirBuild}{name}.toml";
                if (cfg.GetCVar(cVar) && res.TryContentFileRead(path, out var file))
                {
                    cfg.LoadDefaultsFromTomlStream(file);
                    sawmill.Info("Loaded config preset: {Preset}", path);
                }
            }
#pragma warning restore CS8321
        }
    }
}
