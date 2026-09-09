namespace Content.Shared._Funkystation.StationTime;

/// <summary>
/// raised on the clock entity whenever it's toggled
/// </summary>
[ByRefEvent]
public readonly record struct StationClockToggledEvent(bool Enabled);
