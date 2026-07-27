using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Emoting.Components;

/// <summary>
/// Component placed on species that have a tail, allowing them to use tail-specific duo emotes
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class TailDuoEmoteComponent : Component
{
}
