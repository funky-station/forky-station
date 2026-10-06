namespace Content.Shared._Funkystation.Fax;

/// <summary>
/// _Funkystation: Raised on a fax machine entity just before faxecute damage is applied to the entity
/// </summary>
[ByRefEvent]
public readonly record struct FaxecuteFiringEvent(EntityUid Mob);

