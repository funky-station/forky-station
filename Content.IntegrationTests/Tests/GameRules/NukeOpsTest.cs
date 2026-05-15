#nullable enable
using System.Linq;
using Content.IntegrationTests;
using Content.IntegrationTests.Pair;
using Content.Server.Body.Components;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Presets;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Server.RoundEnd;
using Content.Server.Shuttles.Components;
using Content.Shared.CCVar;
using Content.Shared.Damage.Components;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.NukeOps;
using Content.Shared.Roles.Components;
using Content.Shared.Station.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.UnitTesting;

namespace Content.IntegrationTests.Tests.GameRules;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public sealed class NukeOpsTest
{
    private TestPair _pair = default!;

    private static readonly ProtoId<NpcFactionPrototype> SyndicateFaction = "Syndicate";
    private static readonly ProtoId<NpcFactionPrototype> NanotrasenFaction = "NanoTrasen";

    private static readonly string[] NukeOpsAntagRoles =
    [
        "Nukeops",
        "NukeopsMedic",
        "NukeopsCommander",
    ];

    [SetUp]
    public async Task SetUp()
    {
        _pair = await PoolManager.GetServerClient(
            new PoolSettings
            {
                Dirty = true,
                DummyTicker = false,
                Connected = true,
                InLobby = true,
            },
            new NUnitTestContextWrap(TestContext.CurrentContext, TestContext.Out));
    }

    [TearDown]
    public async Task TearDown() => await _pair.CleanReturnAsync();

    [Test]
    public async Task NukeOps_StartsRoundAntagsAndOperativeSpawn()
    {
        var ctx = await StartNukeOpsStandardRoundAsync(_pair);
        try
        {
            var pair = ctx.Pair;
            var server = pair.Server;
            var client = pair.Client;
            var entMan = ctx.EntMan;
            var ticker = ctx.Ticker;
            var mindSys = ctx.MindSys;
            var roleSys = ctx.RoleSys;
            var invSys = ctx.InvSys;
            var factionSys = ctx.FactionSys;
            var player = ctx.Player;
            var dummyEnts = ctx.DummyEnts;

            Assert.That(ticker.RunLevel, Is.EqualTo(GameRunLevel.InRound));
            Assert.That(ticker.PlayerGameStatuses.Values.All(x => x == PlayerGameStatus.JoinedGame));
            Assert.That(client.EntMan.EntityExists(client.AttachedEntity));

            Assert.That(entMan.Count<MapComponent>(), Is.GreaterThan(0));
            Assert.That(entMan.Count<MapGridComponent>(), Is.GreaterThan(0));
            Assert.That(entMan.Count<StationCentcommComponent>(), Is.EqualTo(1));

            Assert.That(entMan.Count<NukeopsRuleComponent>(), Is.EqualTo(1));
            Assert.That(entMan.Count<NukeopsRoleComponent>(), Is.EqualTo(2));
            Assert.That(entMan.Count<NukeOperativeComponent>(), Is.EqualTo(2));
            Assert.That(entMan.Count<NukeOpsShuttleComponent>(), Is.EqualTo(1));

            var mind = mindSys.GetMind(player)!.Value;
            Assert.That(entMan.HasComponent<NukeOperativeComponent>(player));
            Assert.That(roleSys.MindIsAntagonist(mind));
            Assert.That(roleSys.MindHasRole<NukeopsRoleComponent>(mind));
            Assert.That(factionSys.IsMember(player, SyndicateFaction), Is.True);
            Assert.That(factionSys.IsMember(player, NanotrasenFaction), Is.False);
            Assert.That(roleSys.MindGetAllRoleInfo(mind).Count(x => x.Prototype == "NukeopsCommander"), Is.EqualTo(1));

            var dummyMind = mindSys.GetMind(dummyEnts[1])!.Value;
            Assert.That(entMan.HasComponent<NukeOperativeComponent>(dummyEnts[1]));
            Assert.That(roleSys.MindIsAntagonist(dummyMind));
            Assert.That(roleSys.MindHasRole<NukeopsRoleComponent>(dummyMind));
            Assert.That(factionSys.IsMember(dummyEnts[1], SyndicateFaction), Is.True);
            Assert.That(factionSys.IsMember(dummyEnts[1], NanotrasenFaction), Is.False);
            Assert.That(roleSys.MindGetAllRoleInfo(dummyMind).Count(x => x.Prototype == "NukeopsMedic"), Is.EqualTo(1));

            AssertCrewDummy(dummyEnts[0], mindSys, roleSys, factionSys, entMan);
            AssertCrewDummy(dummyEnts[2], mindSys, roleSys, factionSys, entMan);

            Assert.That(entMan.HasComponent<HandsComponent>(player));
            Assert.That(entMan.GetComponent<HandsComponent>(player).Hands.Count, Is.GreaterThan(0));

            var total = 0;
            var enumerator = invSys.GetSlotEnumerator(player);
            while (enumerator.NextItem(out _))
                total++;

            Assert.That(total, Is.GreaterThan(3));

            if (entMan.TryGetComponent<RespiratorComponent>(player, out var resp))
            {
                var totalSeconds = 30;
                var totalTicks = (int)Math.Ceiling(totalSeconds / server.Timing.TickPeriod.TotalSeconds);
                const int increment = 5;
                for (var tick = 0; tick < totalTicks; tick += increment)
                {
                    await pair.RunTicksSync(increment);
                    Assert.That(resp.SuffocationCycles, Is.LessThanOrEqualTo(resp.SuffocationCycleThreshold));
                    Assert.That(entMan.GetComponent<DamageableComponent>(player).TotalDamage, Is.EqualTo(FixedPoint2.Zero));
                }
            }
        }
        finally
        {
            ClearNukeOpsPreset(ctx);
        }
    }

    [Test]
    public async Task NukeOps_TargetStationMapsGridsAndInit()
    {
        var ctx = await StartNukeOpsStandardRoundAsync(_pair);
        try
        {
            var entMan = ctx.EntMan;
            var mapSys = ctx.MapSys;
            var player = ctx.Player;
            var ruleUid = ctx.RuleUid;
            var ruleComp = ctx.RuleComp;
            var gridsRule = ctx.GridsRule;
            var nukieShuttleEnt = ctx.NukieShuttleEnt;
            var nukieMap = ctx.NukieMap;
            var targetMap = ctx.TargetMap;

            foreach (var grid in gridsRule.MapGrids)
            {
                Assert.That(entMan.EntityExists(grid));
                Assert.That(entMan.HasComponent<MapGridComponent>(grid));
            }

            Assert.That(entMan.EntityExists(ruleComp.TargetStation));
            Assert.That(entMan.HasComponent<StationDataComponent>(ruleComp.TargetStation));

            var nukieShuttle = entMan.GetComponent<NukeOpsShuttleComponent>(nukieShuttleEnt);
            Assert.That(nukieShuttle.AssociatedRule, Is.EqualTo(ruleUid));

            EntityUid? nukieStationEnt = null;
            foreach (var grid in gridsRule.MapGrids)
            {
                if (entMan.HasComponent<StationMemberComponent>(grid))
                {
                    nukieStationEnt = grid;
                    break;
                }
            }

            Assert.That(!entMan.EntityExists(nukieStationEnt));
            Assert.That(mapSys.MapExists(gridsRule.Map));

            Assert.That(targetMap, Is.Not.EqualTo(nukieMap));
            Assert.That(entMan.GetComponent<TransformComponent>(player).MapUid, Is.EqualTo(nukieMap));
            Assert.That(entMan.GetComponent<TransformComponent>(nukieShuttleEnt).MapUid, Is.EqualTo(nukieMap));

            Assert.That(mapSys.IsInitialized(nukieMap));
            Assert.That(mapSys.IsInitialized(targetMap));
            Assert.That(mapSys.IsPaused(nukieMap), Is.False);
            Assert.That(mapSys.IsPaused(targetMap), Is.False);

            Assert.That(LifeStage(entMan, player), Is.GreaterThan(EntityLifeStage.Initialized));
            Assert.That(LifeStage(entMan, nukieMap), Is.GreaterThan(EntityLifeStage.Initialized));
            Assert.That(LifeStage(entMan, targetMap), Is.GreaterThan(EntityLifeStage.Initialized));
            Assert.That(LifeStage(entMan, nukieShuttleEnt), Is.GreaterThan(EntityLifeStage.Initialized));
            Assert.That(LifeStage(entMan, ruleComp.TargetStation!.Value), Is.GreaterThan(EntityLifeStage.Initialized));
        }
        finally
        {
            ClearNukeOpsPreset(ctx);
        }
    }

    [Test]
    public async Task NukeOps_RoundEndWhenLastOperativeDeleted()
    {
        var ctx = await StartNukeOpsStandardRoundAsync(_pair);
        try
        {
            var server = ctx.Pair.Server;
            var entMan = ctx.EntMan;
            var roundEndSys = ctx.RoundEndSys;
            var player = ctx.Player;
            var dummyEnts = ctx.DummyEnts;

            var nukies = dummyEnts.Where(ent => entMan.HasComponent<NukeOperativeComponent>(ent)).Append(player).ToArray();

            await server.WaitAssertion(() =>
            {
                for (var i = 0; i < nukies.Length - 1; i++)
                {
                    entMan.DeleteEntity(nukies[i]);
                    Assert.That(roundEndSys.IsRoundEndRequested,
                        Is.False,
                        $"The round ended, but {nukies.Length - i - 1} nukies are still alive!");
                }

                entMan.DeleteEntity(nukies[^1]);

                Assert.That(roundEndSys.IsRoundEndRequested,
                    "All nukies were deleted, but the round didn't end!");
            });
        }
        finally
        {
            ClearNukeOpsPreset(ctx);
        }
    }

    private static EntityLifeStage LifeStage(IEntityManager entMan, EntityUid uid) =>
        entMan.GetComponent<MetaDataComponent>(uid).EntityLifeStage;

    private static void AssertCrewDummy(
        EntityUid ent,
        MindSystem mindSys,
        RoleSystem roleSys,
        NpcFactionSystem factionSys,
        IEntityManager entMan)
    {
        var mindCrew = mindSys.GetMind(ent)!.Value;
        Assert.That(entMan.HasComponent<NukeOperativeComponent>(ent), Is.False);
        Assert.That(roleSys.MindIsAntagonist(mindCrew), Is.False);
        Assert.That(roleSys.MindHasRole<NukeopsRoleComponent>(mindCrew), Is.False);
        Assert.That(factionSys.IsMember(ent, SyndicateFaction), Is.False);
        Assert.That(factionSys.IsMember(ent, NanotrasenFaction), Is.True);
        Assert.That(roleSys.MindGetAllRoleInfo(mindCrew).Any(x => NukeOpsAntagRoles.Contains(x.Prototype)), Is.False);
    }

    private static void ClearNukeOpsPreset(NukeOpsRoundContext ctx) =>
        ctx.Ticker.SetGamePreset((GamePresetPrototype?) null);

    private async Task<NukeOpsRoundContext> StartNukeOpsStandardRoundAsync(TestPair pair)
    {
        var server = pair.Server;
        var client = pair.Client;
        var entMan = server.EntMan;
        var mapSys = server.System<MapSystem>();
        var ticker = server.System<GameTicker>();
        var mindSys = server.System<MindSystem>();
        var roleSys = server.System<RoleSystem>();
        var invSys = server.System<InventorySystem>();
        var factionSys = server.System<NpcFactionSystem>();
        var roundEndSys = server.System<RoundEndSystem>();
        server.CfgMan.SetCVar(CCVars.GridFill, true);

        var dummies = await server.AddDummySessions(3);
        await pair.RunTicksSync(5);

        await pair.SetAntagPreference("NukeopsCommander", true);
        await pair.SetAntagPreference("NukeopsMedic", true, dummies[1].UserId);

        ticker.ToggleReadyAll(true);
        await pair.WaitCommand("forcepreset Nukeops");

        await PoolManager.WaitUntil(server, () =>
            ticker.RunLevel == GameRunLevel.InRound
            && entMan.Count<NukeopsRuleComponent>() == 1
            && client.AttachedEntity != null);

        NukeOpsRoundContext context = default!;
        await server.WaitAssertion(() =>
        {
            var dummyEnts = dummies.Select(x => x.AttachedEntity ?? default).ToArray();
            var player = pair.Player!.AttachedEntity!.Value;

            Assert.That(entMan.EntityExists(player));
            Assert.That(dummyEnts.All(entMan.EntityExists));

            var rule = entMan.AllComponents<NukeopsRuleComponent>().Single();
            var ruleComp = rule.Component;
            var gridsRule = entMan.GetComponent<RuleGridsComponent>(rule.Uid);
            var nukieShuttle = entMan.AllComponents<NukeOpsShuttleComponent>().Single();
            var nukieMap = mapSys.GetMap(gridsRule.Map!.Value);

            Assert.That(entMan.EntityExists(ruleComp.TargetStation));
            var targetStation = entMan.GetComponent<StationDataComponent>(ruleComp.TargetStation!.Value);
            var targetGrid = targetStation.Grids.First();
            var targetMap = entMan.GetComponent<TransformComponent>(targetGrid).MapUid!.Value;

            context = new NukeOpsRoundContext(
                pair,
                entMan,
                mapSys,
                ticker,
                mindSys,
                roleSys,
                invSys,
                factionSys,
                roundEndSys,
                player,
                dummyEnts,
                rule.Uid,
                ruleComp,
                gridsRule,
                nukieShuttle.Uid,
                nukieMap,
                targetMap);
        });

        return context;
    }

    private sealed record NukeOpsRoundContext(
        TestPair Pair,
        IEntityManager EntMan,
        MapSystem MapSys,
        GameTicker Ticker,
        MindSystem MindSys,
        RoleSystem RoleSys,
        InventorySystem InvSys,
        NpcFactionSystem FactionSys,
        RoundEndSystem RoundEndSys,
        EntityUid Player,
        EntityUid[] DummyEnts,
        EntityUid RuleUid,
        NukeopsRuleComponent RuleComp,
        RuleGridsComponent GridsRule,
        EntityUid NukieShuttleEnt,
        EntityUid NukieMap,
        EntityUid TargetMap);
}
