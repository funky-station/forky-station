using Content.Server.Atmos.EntitySystems;
using Content.Shared._Funkystation.StationTime.Components;
using Content.Shared.Temperature;

namespace Content.Server._Funkystation.StationTime;

public sealed partial class StationClockUpdateSystem : EntitySystem
{
    [Dependency] private AtmosphereSystem _atmosphere = null!;
    [Dependency] private SharedTransformSystem _transform = null!;

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<StationClockComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var clock, out var xform))
        {
            if (!clock.ShowTemperature || !clock.Enabled)
                continue;

            clock.Accumulator += frameTime;
            if (clock.Accumulator < clock.TemperatureUpdateInterval)
                continue;
            clock.Accumulator = 0f;

            var coords = _transform.GetGridTilePositionOrDefault((uid, xform));
            var mixture = _atmosphere.GetTileMixture(xform.GridUid, null, coords);
            var newTemp = mixture != null
                ? (float?) TemperatureHelpers.KelvinToCelsius(mixture.Temperature)
                : null;

            if (Math.Abs((newTemp ?? float.MinValue) - (clock.LastTemperatureCelsius ?? float.MinValue)) < 0.05f)
                continue;

            clock.LastTemperatureCelsius = newTemp;
            Dirty(uid, clock);
        }
    }
}
