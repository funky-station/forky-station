using Content.Client._Starfall.Particles;
using Content.Shared._Funkystation.SuitBreach.Components;

namespace Content.Client._Funkystation.SuitBreach.Systems;

public sealed partial class SuitBreachVisualsSystem : EntitySystem
{
    [Dependency] private ParticleSystem _particles = null!;
    [Dependency] private SharedTransformSystem _transform = null!;

    private readonly Dictionary<EntityUid, ActiveEmitter?> _emitters = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SuitBreachedComponent, AfterAutoHandleStateEvent>(OnHandleState);
        SubscribeLocalEvent<SuitBreachedComponent, ComponentShutdown>(OnShutdown);
    }

    private void OnHandleState(Entity<SuitBreachedComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        UpdateVisuals(ent);
    }

    private void OnShutdown(Entity<SuitBreachedComponent> ent, ref ComponentShutdown args)
    {
        if (_emitters.Remove(ent.Owner, out var emitter))
            _particles.RemoveParticle(emitter);
    }

    private void UpdateVisuals(Entity<SuitBreachedComponent> ent)
    {
        var shouldLeak = ent.Comp.IsLeaking;
        var isLeaking = _emitters.ContainsKey(ent.Owner);

        if (shouldLeak && !isLeaking)
        {
            var coords = _transform.GetMapCoordinates(ent.Owner);
            _emitters[ent.Owner] = _particles.SpawnEffect("SuitBreachAirLeak", coords, ent.Owner);
        }
        else if (!shouldLeak && isLeaking)
        {
            if (_emitters.Remove(ent.Owner, out var emitter))
                _particles.RemoveParticle(emitter);
        }
    }

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        foreach (var (uid, emitter) in _emitters)
        {
            if (Deleted(uid) || emitter is not { Exhausted: false })
                continue;

            emitter.MapCoords = _transform.GetMapCoordinates(uid);
        }
    }
}
