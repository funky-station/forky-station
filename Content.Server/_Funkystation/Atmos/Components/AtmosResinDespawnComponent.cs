using Content.Server._Funkystation.Atmos.EntitySystems;

namespace Content.Server._Funkystation.Atmos.Components;

/// <summary>
/// When a TimedDespawnComponent despawns, another one will be spawned in its place.
/// Funky atmos - firefighter backpack
/// </summary>
[RegisterComponent, Access(typeof(AtmosResinDespawnSystem))]
public sealed partial class AtmosResinDespawnComponent : Component
{
}
