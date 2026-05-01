using Robust.Shared.Prototypes;

namespace Content.Shared.Administration.Logs;

/// <summary>
/// Lists entity prototypes whose <c>GhostRole</c> claims are written to player monitoring when taken from ghost.
/// Match is against the entity that owns the ghost role (spawn marker, mob, reinforcement radio, etc.), including inheritance.
/// </summary>
[Prototype("playerMonitoringGhostWatchlist")]
public sealed partial class PlayerMonitoringGhostWatchlistPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Entity prototype IDs to flag when a player claims their ghost role.
    /// </summary>
    [DataField]
    public HashSet<EntProtoId> GhostRoles { get; private set; } = new();
}
