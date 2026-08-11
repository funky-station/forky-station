using Content.Shared._Funkystation.Shuttles.Systems;
using Content.Server._Funkystation.Shuttles.Components;
using Content.Server._Funkystation.Shuttles.Events;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map;

namespace Content.Server._Funkystation.Shuttles.Systems;

public sealed partial class TraderShuttleSystem : SharedTraderShuttleSystem
{

    [Dependency] private SharedMapSystem _mapSystem = default!;
    [Dependency] private MapLoaderSystem _loader = default!;
    [Dependency] private ShuttleSystem _shuttles = default!;
    [Dependency] private StationSystem _station = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TraderShuttleComponent, CallTraderShuttleEvent>(CallTraderShuttle);
    }

    private void CallTraderShuttle(EntityUid ent, TraderShuttleComponent comp, ref CallTraderShuttleEvent args)
    {
        if (!TryComp<StationTraderComponent>(args.Station, out var stationTrader))
        {
            return;
        }
        if (_station.GetLargestGrid(args.Station) is not { } stationGrid)
            return;
        _mapSystem.CreateMap(out var dummyMapId);

        var minOffset = stationTrader.TraderArrivalMinOffset;
        var maxOffset = stationTrader.TraderArrivalMaxOffset;

        if (_loader.TryLoadGrid(dummyMapId, comp.ShuttlePath, out var shuttle))
        {
            comp.Shuttle = shuttle.Value;
            var traderComp = EnsureComp<TraderShuttleComponent>(comp.Shuttle);
            traderComp.Station = args.Station;
            _shuttles.TryFTLProximityOffset(comp.Shuttle, stationGrid, minOffset, maxOffset);
            // add the time for traders to leave and stuff
        }
        // here will be code to cleanup the map
    }
}
