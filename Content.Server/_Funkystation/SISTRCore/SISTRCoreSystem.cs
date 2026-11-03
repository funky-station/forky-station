using System.Linq;
using Content.Server.Power.EntitySystems;
using Content.Server.Station.Components;
using Content.Server.StationEvents.Components;
using Content.Shared._Funkystation.CCVar;
using Content.Shared.Station.Components;
using Robust.Shared.Configuration;

namespace Content.Server._Funkystation.SistrCore;

/// <summary>
/// tracks whether the physical sis/tr core is alive and powered
/// </summary>
public sealed partial class SistrCoreSystem : EntitySystem
{
    [Dependency] private PowerReceiverSystem _power = null!;
    [Dependency] private IConfigurationManager _cfg = null!;

    [Dependency] private EntityQuery<StationDataComponent> _stationDataQuery;
    [Dependency] private EntityQuery<StationEventComponent> _stationEventQuery;

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
        return _stationDataQuery.TryComp(station, out var data) && data.Grids.Any(GridHasFunctionalCore);
    }

    public bool ShouldStationEventBeAnnounced(EntityUid eventUid, bool ending = false)
    {
        var canAnnounce = true;
        var stationEventComp = _stationEventQuery.Comp(eventUid);
        if (_cfg.GetCVar(SistrCVars.SistrRequired) && (ending
                ? stationEventComp.EndAnnouncementSender == "chat-manager-sender-sistr"
                : stationEventComp.StartAnnouncementSender == "chat-manager-sender-sistr"))
        {
            var sistrUp = false;
            var query = EntityQueryEnumerator<StationEventEligibleComponent>();
            while (query.MoveNext(out var stationUid, out _))
            {
                if (StationHasFunctionalCore(stationUid))
                {
                    sistrUp = true;
                    break;
                }
            }

            canAnnounce = sistrUp;
        }

        return canAnnounce;
    }
}
