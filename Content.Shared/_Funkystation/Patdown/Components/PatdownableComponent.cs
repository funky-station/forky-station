using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Patdown.Components;

/// <summary>
/// marks an entity as searchable via patdown
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PatdownableComponent : Component
{
    /// <summary>
    /// how long the patdown doAfter takes.
    /// </summary>
    [DataField]
    public TimeSpan Delay = TimeSpan.FromSeconds(8);
}
