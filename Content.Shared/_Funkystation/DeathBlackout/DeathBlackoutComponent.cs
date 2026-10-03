using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.DeathBlackout;

/// <summary>
/// added to a dead mob that had a mind
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, UnsavedComponent]
public sealed partial class DeathBlackoutComponent : Component
{
    /// <summary>
    /// time the blackout ends at
    /// </summary>
    [AutoNetworkedField]
    public TimeSpan EndTime;
}
