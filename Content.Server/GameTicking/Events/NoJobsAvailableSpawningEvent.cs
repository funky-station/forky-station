using Robust.Shared.Player;

namespace Content.Server._Funkystation.GameTicking.Events;

/// <summary>
/// Raised on players who attempt to spawn in but fail to get a job, due to there not being any job slots available.
/// </summary>
/// <param name="Player">The session that could not be assigned a job.</param>
/// <param name="LateJoin">
/// True when this attempt comes from <see cref="Content.Server.GameTicking.GameTicker"/>'s late-join <c>SpawnPlayer</c> path
/// (mid-round join / reconnect spawn attempt). False for the initial round-start spawn batch.
/// </param>
public readonly record struct NoJobsAvailableSpawningEvent(ICommonSession Player, bool LateJoin = false); // Funky - LateJoin flag for admin tracking purposes
