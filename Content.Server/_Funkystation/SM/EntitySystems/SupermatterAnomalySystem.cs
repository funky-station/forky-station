using System.Numerics;
using Content.Shared._Funkystation.SM.Components;
using Content.Server.Anomaly.Components;
using Content.Shared._Funkystation.SM.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Funkystation.SM;
using Content.Shared.Physics;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Random;

namespace Content.Server._Funkystation.SM.EntitySystems;

/// <summary>
/// Spawns anomalies near an energized supermatter on an interval.
/// </summary>
public sealed partial class SupermatterAnomalySystem : SharedSupermatterAnomalySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SharedSupermatterComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var sm, out var xform))
        {
            if (sm.Delaminated || sm.Delamming || HasComp<SupermatterShardComponent>(uid))
                continue;

            if (MathF.Abs(sm.Power) < sm.AnomalyMinPower || MathF.Abs(sm.Conductivity) < sm.AnomalyMinConductivity)
                continue;

            if (xform.GridUid is not { } gridUid || !TryComp<MapGridComponent>(gridUid, out var grid))
                continue;

            sm.AnomalyCooldown -= frameTime;
            if (sm.AnomalyCooldown > 0)
                continue;

            sm.AnomalyCooldown = sm.AnomalySpawnInterval;

            if (!TrySpawnNear(uid, sm, gridUid, grid, xform))
                sm.AnomalyCooldown = MathF.Min(sm.AnomalySpawnInterval, 5f);
        }
    }

    private bool TrySpawnNear(EntityUid smUid, SharedSupermatterComponent sm, EntityUid gridUid, MapGridComponent grid, TransformComponent smXform)
    {
        var center = _map.TileIndicesFor(gridUid, grid, smXform.Coordinates);
        var proto = sm.AnomalySpawnPrototype.Id;
        if (string.IsNullOrEmpty(proto))
            return false;

        for (var attempt = 0; attempt < 24; attempt++)
        {
            var angle = _random.NextFloat() * MathF.PI * 2f;
            var dist = _random.NextFloat(sm.AnomalySpawnMinRadius, sm.AnomalySpawnMaxRadius);
            var offset = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * dist;
            var tile = new Vector2i(
                center.X + (int)MathF.Round(offset.X),
                center.Y + (int)MathF.Round(offset.Y));

            if (_atmosphere.IsTileSpace(gridUid, smXform.MapUid, tile) ||
                _atmosphere.IsTileAirBlockedCached(gridUid, tile))
                continue;

            var physQuery = GetEntityQuery<PhysicsComponent>();
            var valid = true;
            foreach (var ent in _map.GetAnchoredEntities(gridUid, grid, tile))
            {
                if (!physQuery.TryGetComponent(ent, out var body))
                    continue;
                if (body.BodyType != BodyType.Static ||
                    !body.Hard ||
                    (body.CollisionLayer & (int)CollisionGroup.Impassable) == 0)
                    continue;
                valid = false;
                break;
            }

            if (!valid)
                continue;

            var anti = AllEntityQuery<AntiAnomalyZoneComponent, TransformComponent>();
            var mapPos = _transform.ToMapCoordinates(_map.GridTileToLocal(gridUid, grid, tile));
            while (anti.MoveNext(out _, out var zone, out var zx))
            {
                if (zx.MapID != mapPos.MapId)
                    continue;
                var zpos = _transform.GetWorldPosition(zx);
                if ((zpos - mapPos.Position).LengthSquared() < zone.ZoneRadius * zone.ZoneRadius)
                {
                    valid = false;
                    break;
                }
            }

            if (!valid)
                continue;

            var coords = _map.GridTileToLocal(gridUid, grid, tile);
            Spawn(proto, coords);
            return true;
        }

        return false;
    }
}
