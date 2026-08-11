namespace Content.Server._Funkystation.Shuttles.Events;

/// <summary>
/// Raised when a station calls a trader shuttle with the FTL comms computer.
/// </summary>
[ByRefEvent]
public readonly record struct CallTraderShuttleEvent(EntityUid Shuttle, EntityUid Station);
