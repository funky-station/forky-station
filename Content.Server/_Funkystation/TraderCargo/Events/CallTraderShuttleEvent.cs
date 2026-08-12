namespace Content.Server._Funkystation.TraderCargo.Events;

/// <summary>
/// Raised when a station calls a trader shuttle with the FTL comms computer.
/// </summary>
[ByRefEvent]
public readonly record struct CallTraderShuttleEvent(EntityUid Shuttle, EntityUid Station);
