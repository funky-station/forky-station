using Content.Shared._Funkystation.StationTime.Components;
using Content.Shared.Station.Components;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.StationTime;

public sealed partial class StationTimeSetupSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = null!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<StationDataComponent>();
        while (query.MoveNext(out var uid, out _))
        {
            if (HasComp<StationTimeComponent>(uid))
                continue;

            var comp = AddComp<StationTimeComponent>(uid);
            comp.RealUtcTicksAtSync = DateTime.UtcNow.Ticks;
            comp.CurTimeAtSync = _timing.CurTime;
        }
    }
}
