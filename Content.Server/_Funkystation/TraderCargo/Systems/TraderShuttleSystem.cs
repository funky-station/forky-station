using Content.Shared._Funkystation.TraderCargo.Systems;
using Content.Server._Funkystation.TraderCargo.Components;
using Content.Server._Funkystation.TraderCargo.Events;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.TraderCargo.Systems;

public sealed partial class TraderShuttleSystem : SharedTraderShuttleSystem
{

    [Dependency] private SharedMapSystem _mapSystem = default!;
    [Dependency] private MapLoaderSystem _loader = default!;
    [Dependency] private ShuttleSystem _shuttles = default!;
    [Dependency] private StationSystem _station = default!;
    [Dependency] private IGameTiming _timing = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StationTraderComponent, CallTraderShuttleEvent>(CallTraderShuttle);
    }

    private void CallTraderShuttle(EntityUid ent, StationTraderComponent comp, ref CallTraderShuttleEvent args)
    {
        if (!TryComp<TraderShuttleComponent>(args.Shuttle, out var traderShuttle) ||
        _station.GetLargestGrid(args.Station) is not { } stationGrid)
            return;

        if (comp.CurrentTraders >= comp.MaxTraders)
            return;

        var dummyMapEntity = _mapSystem.CreateMap(out var dummyMapId);

        var minOffset = comp.TraderArrivalMinOffset;
        var maxOffset = comp.TraderArrivalMaxOffset;


        if (_loader.TryLoadGrid(dummyMapId, traderShuttle.ShuttlePath, out var shuttle))
        {
            traderShuttle.Shuttle = shuttle.Value;
            var traderComp = EnsureComp<TraderShuttleComponent>(traderShuttle.Shuttle);
            traderComp.Station = args.Station;

            if (_shuttles.TryFTLProximityOffset(traderShuttle.Shuttle, stationGrid, minOffset, maxOffset))
            {
                traderComp.TraderLeaveTime = _timing.CurTime + traderShuttle.TraderLeaveTime;
                comp.CurrentTraders++;
            }
        }
        var timer = AddComp<TimedDespawnComponent>(dummyMapEntity);
        timer.Lifetime = 5f;
    }
}
