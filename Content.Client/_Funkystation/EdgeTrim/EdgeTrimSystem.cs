using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using static Robust.Client.GameObjects.SpriteComponent;

namespace Content.Client._FunkyStation.EdgeTrim
{
    [UsedImplicitly]
    public sealed partial class EdgeTrimSystem : EntitySystem
    {
        [Dependency] private SharedMapSystem _mapSystem = default!;
        [Dependency] private SpriteSystem _sprite = default!;
        [Dependency] private EntityQuery<EdgeTrimComponent> _edgeTrimQuery = default!;
        [Dependency] private EntityQuery<SpriteComponent> _spriteQuery = default!;

        private readonly Queue<EntityUid> _dirtyEntities = new();
        private readonly Queue<EntityUid> _anchorChangedEntities = new();

        private int _generation;

        public void SetEnabled(EntityUid uid, bool value, EdgeTrimComponent? component = null)
        {
            if (!Resolve(uid, ref component, false) || value == component.Enabled)
                return;

            component.Enabled = value;
            DirtyNeighbours(uid, component);
        }

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<EdgeTrimComponent, AnchorStateChangedEvent>(OnAnchorChanged);
            SubscribeLocalEvent<EdgeTrimComponent, ComponentShutdown>(OnShutdown);
            SubscribeLocalEvent<EdgeTrimComponent, ComponentStartup>(OnStartup);
        }

        private void OnStartup(EntityUid uid, EdgeTrimComponent component, ComponentStartup args)
        {
            var xform = Transform(uid);
            if (xform.Anchored)
            {
                component.LastPosition = TryComp<MapGridComponent>(xform.GridUid, out var grid)
                    ? (xform.GridUid.Value, _mapSystem.TileIndicesFor(xform.GridUid.Value, grid, xform.Coordinates))
                    : (null, new Vector2i(0, 0));

                DirtyNeighbours(uid, component);
            }

            if (component.Mode != EdgeTrimMode.Default || !TryComp(uid, out SpriteComponent? sprite))
                return;

            SetCornerLayers((uid, sprite), component);

            if (component.Shader != null)
            {
                sprite.LayerSetShader(CornerLayers.SE, component.Shader);
                sprite.LayerSetShader(CornerLayers.NE, component.Shader);
                sprite.LayerSetShader(CornerLayers.NW, component.Shader);
                sprite.LayerSetShader(CornerLayers.SW, component.Shader);
            }
        }
        private void OnShutdown(EntityUid uid, EdgeTrimComponent component, ComponentShutdown args)
        {
            _dirtyEntities.Enqueue(uid);
            DirtyNeighbours(uid, component);
        }

        private void SetCornerLayers(Entity<SpriteComponent?> sprite, EdgeTrimComponent component)
        {
            _sprite.LayerMapRemove(sprite, CornerLayers.SE);
            _sprite.LayerMapRemove(sprite, CornerLayers.NE);
            _sprite.LayerMapRemove(sprite, CornerLayers.NW);
            _sprite.LayerMapRemove(sprite, CornerLayers.SW);

            var state0 = $"{component.StateBase}-smooth0";
            _sprite.LayerMapSet(sprite, CornerLayers.SE, _sprite.AddRsiLayer(sprite, state0));
            _sprite.LayerSetDirOffset(sprite, CornerLayers.SE, DirectionOffset.None);
            _sprite.LayerMapSet(sprite, CornerLayers.NE, _sprite.AddRsiLayer(sprite, state0));
            _sprite.LayerSetDirOffset(sprite, CornerLayers.NE, DirectionOffset.CounterClockwise);
            _sprite.LayerMapSet(sprite, CornerLayers.NW, _sprite.AddRsiLayer(sprite, state0));
            _sprite.LayerSetDirOffset(sprite, CornerLayers.NW, DirectionOffset.Flip);
            _sprite.LayerMapSet(sprite, CornerLayers.SW, _sprite.AddRsiLayer(sprite, state0));
            _sprite.LayerSetDirOffset(sprite, CornerLayers.SW, DirectionOffset.Clockwise);
        }

        public override void FrameUpdate(float frameTime)
        {
            base.FrameUpdate(frameTime);

            // first process anchor state changes.
            while (_anchorChangedEntities.TryDequeue(out var uid))
            {
                if (!TryComp(uid, out TransformComponent? xform))
                    continue;

                if (xform.MapID == MapId.Nullspace)
                {
                    // in null-space. Almost certainly because it left PVS. If something ever gets sent to null-space
                    // for reasons other than this (or entity deletion), then maybe we still need to update ex-neighbor
                    // smoothing here.
                    continue;
                }

                DirtyNeighbours(uid, comp: null, xform);
            }

            // Next, update actual sprites.
            if (_dirtyEntities.Count == 0)
                return;

            _generation += 1;

            // Performance: This could be spread over multiple updates, or made parallel.
            while (_dirtyEntities.TryDequeue(out var uid))
            {
                CalculateNewSprite(uid);
            }
        }

        private void CalculateNewSprite(EntityUid uid, EdgeTrimComponent? smooth = null)
        {
            TransformComponent? xform;
            Entity<MapGridComponent>? gridEntity = null;

            // The generation check prevents updating an entity multiple times per tick.
            // As it stands now, it's totally possible for something to get queued twice.
            // Generation on the component is set after an update so we can cull updates that happened this generation.
            if (!_edgeTrimQuery.Resolve(uid, ref smooth, false)
                || smooth.Mode == EdgeTrimMode.NoSprite
                || smooth.UpdateGeneration == _generation
                || !smooth.Enabled
                || !smooth.Running)
            {
                return;
            }

            xform = Transform(uid);
            smooth.UpdateGeneration = _generation;

            if (!_spriteQuery.TryGetComponent(uid, out var sprite))
            {
                Log.Error($"Encountered a edge-trimming entity without a sprite: {ToPrettyString(uid)}");
                RemCompDeferred(uid, smooth);
                return;
            }

            var spriteEnt = (uid, sprite);

            if (xform.Anchored)
            {
                if (TryComp(xform.GridUid, out MapGridComponent? grid))
                {
                    gridEntity = (xform.GridUid.Value, grid);
                }
                else
                {
                    Log.Error($"Failed to calculate EdgeTrimComponent sprite in {uid} because grid {xform.GridUid} was missing.");
                    return;
                }
            }

            CalculateNewSpriteCorners(gridEntity, smooth, spriteEnt, xform);
        }

        private void CalculateNewSpriteCorners(Entity<MapGridComponent>? gridEntity, EdgeTrimComponent smooth, Entity<SpriteComponent> spriteEnt, TransformComponent xform)
        {
            var (cornerSE, cornerNE, cornerNW, cornerSW) = gridEntity == null
                ? (0, 0, 0, 0)
                : CalculateCornerFill(gridEntity.Value, smooth, xform);

            // Maybe figure out how to do this better later
            // This will currently re-calculate the sprite bounding box 4 times.
            // It will also result in 4-8 sprite update events being raised when it only needs to be 1-2.
            // At the very least each event currently only queues a sprite for updating.
            // Could definitely be better, the sprite component is certainly interesting.
            _sprite.LayerSetRsiState(spriteEnt.AsNullable(), CornerLayers.SE, $"{smooth.StateBase}-{_trimLookup[cornerSE]}");
            _sprite.LayerSetRsiState(spriteEnt.AsNullable(), CornerLayers.NE, $"{smooth.StateBase}-{_trimLookup[cornerNE]}");
            _sprite.LayerSetRsiState(spriteEnt.AsNullable(), CornerLayers.NW, $"{smooth.StateBase}-{_trimLookup[cornerNW]}");
            _sprite.LayerSetRsiState(spriteEnt.AsNullable(), CornerLayers.SW, $"{smooth.StateBase}-{_trimLookup[cornerSW]}");
        }

        private int TrimEntity(EdgeTrimComponent smooth, AnchoredEntitiesEnumerator candidates)
        {
            while (candidates.MoveNext(out var entity))
            {
                if (_edgeTrimQuery.TryGetComponent(entity, out var other) && other.SmoothKey != null
                                                                          && other.Enabled)
                {
                    if (other.SmoothKey == smooth.SmoothKey || smooth.AdditionalKeys.Contains(other.SmoothKey))
                    {
                        return 1; // Smooths to
                    }

                    if (smooth.EdgeKeys.Contains(other.SmoothKey))
                    {
                        return 2; // Trims against
                    }
                }
            }
            return 0; // Ignores
        }

        private (int se, int ne, int nw, int sw) CalculateCornerFill(Entity<MapGridComponent> gridEntity, EdgeTrimComponent smooth, TransformComponent xform)
        {
            var gridUid = gridEntity.Owner;
            var grid = gridEntity.Comp;

            int[] keys = new int[8];
            var pos = _mapSystem.TileIndicesFor(gridUid, grid, xform.Coordinates);
            keys[0] = TrimEntity(smooth, _mapSystem.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.South)));
            keys[1] = TrimEntity(smooth, _mapSystem.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.SouthEast)));
            keys[2] = TrimEntity(smooth, _mapSystem.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.East)));
            keys[3] = TrimEntity(smooth, _mapSystem.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.NorthEast)));
            keys[4] = TrimEntity(smooth, _mapSystem.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.North)));
            keys[5] = TrimEntity(smooth, _mapSystem.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.NorthWest)));
            keys[6] = TrimEntity(smooth, _mapSystem.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.West)));
            keys[7] = TrimEntity(smooth, _mapSystem.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.SouthWest)));

            int cornerSE = keys[0] + 3 * keys[1] + 9 * keys[2];
            int cornerNE = keys[2] + 3 * keys[3] + 9 * keys[4];
            int cornerNW = keys[4] + 3 * keys[5] + 9 * keys[6];
            int cornerSW = keys[6] + 3 * keys[7] + 9 * keys[0];

            switch (xform.LocalRotation.GetCardinalDir())
            {
                case Direction.East:
                    return (cornerNE, cornerNW, cornerSW, cornerSE);
                case Direction.North:
                    return (cornerNW, cornerSW, cornerSE, cornerNE);
                case Direction.West:
                    return (cornerSW, cornerSE, cornerNE, cornerNW);
                default:
                    return (cornerSE, cornerNE, cornerNW, cornerSW);
            }
        }

        private void DirtyNeighbours(EntityUid uid, EdgeTrimComponent? comp = null, TransformComponent? transform = null)
        {
            if (!_edgeTrimQuery.Resolve(uid, ref comp) || !comp.Running)
                return;

            _dirtyEntities.Enqueue(uid);

            if (!Resolve(uid, ref transform))
                return;

            Vector2i pos;

            EntityUid entityUid;

            if (transform.Anchored && TryComp<MapGridComponent>(transform.GridUid, out var grid))
            {
                entityUid = transform.GridUid.Value;
                pos = _mapSystem.CoordinatesToTile(transform.GridUid.Value, grid, transform.Coordinates);
            }
            else
            {
                // Entity is no longer valid, update around the last position it was at.
                if (comp.LastPosition is not (EntityUid gridId, Vector2i oldPos))
                    return;

                if (!TryComp(gridId, out grid))
                    return;

                entityUid = gridId;
                pos = oldPos;
            }

            // Just taken from IconSmoothing
            DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(entityUid, grid, pos + new Vector2i(1, 0)));
            DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(entityUid, grid, pos + new Vector2i(-1, 0)));
            DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(entityUid, grid, pos + new Vector2i(0, 1)));
            DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(entityUid, grid, pos + new Vector2i(0, -1)));
            DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(entityUid, grid, pos + new Vector2i(1, 1)));
            DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(entityUid, grid, pos + new Vector2i(-1, -1)));
            DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(entityUid, grid, pos + new Vector2i(-1, 1)));
            DirtyEntities(_mapSystem.GetAnchoredEntitiesEnumerator(entityUid, grid, pos + new Vector2i(1, -1)));
        }

        private void DirtyEntities(AnchoredEntitiesEnumerator entities)
        {
            // Instead of doing HasComp -> Enqueue -> TryGetComp, we will just enqueue all entities. Generally when
            // dealing with walls neighboring anchored entities will also be walls, and in those instances that will
            // require one less component fetch/check.
            while (entities.MoveNext(out var entity))
            {
                _dirtyEntities.Enqueue(entity.Value);
            }
        }

        private void OnAnchorChanged(EntityUid uid, EdgeTrimComponent component, ref AnchorStateChangedEvent args)
        {
            if (!args.Detaching)
                _anchorChangedEntities.Enqueue(uid);
        }

        // My string array of doom and despair (check the edge trim template)
        private string[] _trimLookup =
        {
            "smooth0", // First nine entries, top left key is null
            "smooth4",
            "edge0",
            "smooth2",
            "smooth6",
            "edge0",
            "smooth2",
            "smooth6",
            "edge0",
            "smooth1", // Second nine entries, top left key is smooth
            "smooth5",
            "edge6",
            "smooth3",
            "smooth7",
            "edge3",
            "smooth3",
            "edge8",
            "edge3",
            "edge1", // Last nine entries, top left key is edge
            "edge5",
            "edge7",
            "edge1",
            "edge2",
            "edge4",
            "edge1",
            "edge2",
            "edge4"
        };

        private enum CornerLayers : byte
        {
            SE,
            NE,
            NW,
            SW,
        }
    }
}


