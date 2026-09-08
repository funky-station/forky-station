using System.Numerics;
using Content.Shared._Starfall.Particles;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Components;
using Robust.Client.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._Starfall.Particles;

/// <summary>
/// Spawns dust particle puffs under entities' feet when they run.
/// </summary>
public sealed partial class FootstepDustSystem : EntitySystem
{
    [Dependency] private ParticleSystem _particles = default!;
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IEyeManager _eye = default!;

    // funky, a few loose pixels that scatter away from the feet alongside the dust puff
    private static readonly ProtoId<ParticleEffectPrototype> PixelDriftEffect = "SprintPixelDrift";

    // Tracks last spawn position for each entity to avoid spam
    private readonly Dictionary<EntityUid, (MapCoordinates Pos, TimeSpan Time)> _lastDust = new();

    private const float MinDistanceBetweenDust = 0.8f; // tiles
    private const float MinTimeBetweenDust = 0.15f; // seconds

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;
        var toRemove = new List<EntityUid>();

        var movers = EntityQueryEnumerator<InputMoverComponent, TransformComponent, MobStateComponent>();
        while (movers.MoveNext(out var uid, out var mover, out var xform, out var mobState))
        {
            // Only spawn dust when sprinting (holding Walk key), actively moving, and alive
            if (!mover.Sprinting || !mover.HasDirectionalMovement || mobState.CurrentState != Shared.Mobs.MobState.Alive)
                continue;

            var pos = _transform.GetMapCoordinates(uid, xform);

            // The feet offset is screen-down (0, -0.75) — rotate it into world space
            // so it always points toward the bottom of the screen regardless of grid rotation
            var eyeRot = -(float)_eye.CurrentEye.Rotation;
            var cosE = MathF.Cos(eyeRot);
            var sinE = MathF.Sin(eyeRot);
            const float feetOffset = -0.55f;
            var worldFeetOffset = new Vector2(-feetOffset * sinE, feetOffset * cosE);
            var feetPos = new MapCoordinates(pos.Position + worldFeetOffset, pos.MapId);

            // Check if enough time AND distance has passed since last dust
            if (_lastDust.TryGetValue(uid, out var last))
            {
                var timeSince = (curTime - last.Time).TotalSeconds;
                var distSince = (feetPos.Position - last.Pos.Position).Length();

                if (timeSince < MinTimeBetweenDust || distSince < MinDistanceBetweenDust)
                    continue;
            }

            // funky, a couple stray pixels that drift off from the same spot
            _particles.SpawnEffect(PixelDriftEffect, feetPos);

            _lastDust[uid] = (feetPos, curTime);
        }

        // Cleanup old tracked entities
        foreach (var (uid, _) in _lastDust)
        {
            if (!Exists(uid))
                toRemove.Add(uid);
        }

        foreach (var uid in toRemove)
        {
            _lastDust.Remove(uid);
        }
    }
}
