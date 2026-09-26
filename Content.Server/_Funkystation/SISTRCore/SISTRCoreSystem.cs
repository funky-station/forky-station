using System.Linq;
using Content.Server.Power.EntitySystems;
using Content.Shared.Station.Components;

namespace Content.Server._Funkystation.SistrCore;

/// <summary>
/// tracks whether the physical sis/tr core is alive and powered
/// </summary>
public sealed partial class SistrCoreSystem : EntitySystem
{
    [Dependency] private PowerReceiverSystem _power = null!;

    private bool GridHasFunctionalCore(EntityUid grid)
    {
        var query = EntityQueryEnumerator<SistrCoreComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out _, out var xform))
        {
            if (xform.GridUid != grid)
                continue;

            if (!_power.IsPowered(uid))
                continue;

            return true;
        }

        return false;
    }

    // this is some bullshit LOL
    public bool StationHasFunctionalCore(EntityUid station)
    {
        return TryComp<StationDataComponent>(station, out var data) && data.Grids.Any(GridHasFunctionalCore);
    }
}
