using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.SM.Components;

/// <summary>
/// Marks a supermatter monitoring console; server tracks linked crystals on the same grid.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SupermatterConsoleComponent : Component
{
}
