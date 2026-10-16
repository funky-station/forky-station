using Content.Shared._Funkystation.SM.Components;
using Content.Server._Funkystation.SM.Events;
using Content.Server.Explosion.EntitySystems;
using Content.Shared._Funkystation.SM;
using Robust.Shared.Map;

namespace Content.Server._Funkystation.SM.EntitySystems;

public sealed class SupermatterDelamSystem : EntitySystem
{
    [Dependency] private readonly ExplosionSystem _explosion = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<SharedSupermatterComponent, SupermatterDelaminationEvent>(OnDelam);
    }

    private void OnDelam(EntityUid uid, SharedSupermatterComponent sm, ref SupermatterDelaminationEvent args)
    {
        var coords = Transform(uid).Coordinates;
        var mapCoords = _transform.ToMapCoordinates(coords);

        switch (args.DelamType)
        {
            case DelamType.Singulo:
                TriggerSingularity(uid, sm, coords);
                break;
            case DelamType.Tesla:
                TriggerTesla(uid, sm, coords);
                break;
            case DelamType.Explosion:
                TriggerExplosion(uid, sm, mapCoords);
                break;
            case DelamType.Cascade:
                TriggerCascade(uid, sm, mapCoords);
                break;
        }

        QueueDel(uid);
    }

    private void TriggerSingularity(EntityUid _, SharedSupermatterComponent sm, EntityCoordinates coords)
    {
        Spawn(sm.DelamSingularityPrototype, coords);
    }

    private void TriggerTesla(EntityUid _, SharedSupermatterComponent sm, EntityCoordinates coords)
    {
        Spawn(sm.DelamTeslaPrototype, coords);
    }

    private void TriggerExplosion(EntityUid uid, SharedSupermatterComponent sm, MapCoordinates mapCoords)
    {
        _explosion.QueueExplosion(
            mapCoords,
            sm.DelamExplosionPrototype,
            sm.DelamExplosionTotalIntensity,
            sm.DelamExplosionSlope,
            sm.DelamExplosionMaxTileIntensity,
            uid);
    }

    private void TriggerCascade(EntityUid uid, SharedSupermatterComponent sm, MapCoordinates mapCoords)
    {
        var mult = sm.DelamCascadeIntensityMultiplier;
        _explosion.QueueExplosion(
            mapCoords,
            sm.DelamExplosionPrototype,
            sm.DelamExplosionTotalIntensity * mult,
            sm.DelamExplosionSlope,
            sm.DelamExplosionMaxTileIntensity * mult,
            uid);
    }
}
