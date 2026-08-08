using Robust.Shared.Timing;

namespace Content.Shared._Funkystation.StationTime.EntitySystems;

public sealed partial class StationTimeSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = null!;

    // single call every other system should use instead of round time
    public DateTime GetStationTime(Entity<Components.StationTimeComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return default;

        var syncedUtc = new DateTime(Math.Max(ent.Comp.RealUtcTicksAtSync, DateTime.MinValue.Ticks), DateTimeKind.Utc);
        var elapsed = _timing.CurTime - ent.Comp.CurTimeAtSync;

        var ticks = syncedUtc.Ticks + elapsed.Ticks;
        if (ticks < DateTime.MinValue.Ticks)
            ticks = DateTime.MinValue.Ticks;
        if (ticks > DateTime.MaxValue.Ticks)
            ticks = DateTime.MaxValue.Ticks;

        var realNow = new DateTime(ticks, DateTimeKind.Utc);
        // it's 2984
        return new DateTime(2984,
            realNow.Month,
            realNow.Day,
            realNow.Hour,
            realNow.Minute,
            realNow.Second,
            DateTimeKind.Utc);
    }

    public string FormatTime(DateTime dt, bool includeSeconds = false)
    {
        return dt.ToString(includeSeconds ? "HH:mm:ss" : "HH:mm");
    }

    public string FormatDate(DateTime dt)
    {
        return dt.ToString("yyyy-MM-dd");
    }

    public string FormatTimestamp(DateTime dt)
    {
        return $"{FormatDate(dt)} {FormatTime(dt, includeSeconds: true)}";
    }
}
