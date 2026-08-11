using Content.Shared._Funkystation.Shuttles.Systems;
using Content.Server._Funkystation.Shuttles.Components;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map;

namespace Content.Server._Funkystation.Shuttles.Systems;

public sealed partial class TraderShuttleSystem : SharedTraderShuttleSystem
{
    [Dependency] private SharedMapSystem _mapSystem = default!;
    [Dependency] private MapLoaderSystem _loader = default!;
    public override void Initialize()
    {
        base.Initialize();
    }

    private void CallTraderShuttle(EntityUid ent, TraderShuttleComponent comp)
    {
        var dummyMapEntity = _mapSystem.CreateMap(out var dummyMapId);

        if (_loader.TryLoadGrid(dummyMapId, comp.ShuttlePath, out var shuttle))
        {

        }
    }
}
