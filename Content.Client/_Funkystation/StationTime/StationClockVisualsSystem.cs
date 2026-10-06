using Content.Shared._Funkystation.StationTime.Components;
using Content.Shared._Funkystation.StationTime.EntitySystems;
using Content.Shared.Station;
using Robust.Client.GameObjects;

namespace Content.Client._Funkystation.StationTime;

public sealed partial class StationClockVisualsSystem : EntitySystem
{
    [Dependency] private StationTimeSystem _stationTime = null!;
    [Dependency] private  SharedStationSystem _station = null!;
    [Dependency] private SpriteSystem _sprite = null!;

    private float _accumulator;
    private const float CheckInterval = 1f;

    public override void Update(float frameTime)
    {
        _accumulator += frameTime;
        if (_accumulator < CheckInterval)
            return;
        _accumulator = 0f;

        var query = EntityQueryEnumerator<StationClockComponent, SpriteComponent>();
        while (query.MoveNext(out var uid, out var clock, out var sprite))
        {
            ApplyEnabledState(uid, clock, sprite);

            if (!clock.Enabled || clock.ScreenStates.Count == 0)
                continue;

            if (!_sprite.LayerMapTryGet((uid, sprite), "screen", out _, logMissing: false))
                continue;

            var station = _station.GetOwningStation(uid);
            if (station is not { } stationEnt || !TryComp<StationTimeComponent>(stationEnt, out var timeComp))
                continue;

            var time = _stationTime.GetStationTime((stationEnt, timeComp));
            var index = time.Minute % clock.ScreenStates.Count;

            _sprite.LayerSetRsiState((uid, sprite), "screen", clock.ScreenStates[index]);
        }
    }

    private void ApplyEnabledState(EntityUid uid, StationClockComponent clock, SpriteComponent sprite)
    {
        if (_sprite.LayerMapTryGet((uid, sprite), "screen", out _, logMissing: false))
            _sprite.LayerSetVisible((uid, sprite), "screen", clock.Enabled);

        if (_sprite.LayerMapTryGet((uid, sprite), "base", out _, logMissing: false))
            _sprite.LayerSetAutoAnimated((uid, sprite), "base", clock.Enabled);
    }
}
