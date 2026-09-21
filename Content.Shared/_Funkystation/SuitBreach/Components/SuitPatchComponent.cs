using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.SuitBreach.Components;

/// <summary>
/// an item that can be used to patch a breached suit
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SuitPatchComponent : Component
{
    /// <summary>
    /// doafter length when patching your own suit
    /// </summary>
    [DataField]
    public TimeSpan SelfApplyDelay = TimeSpan.FromSeconds(20);

    /// <summary>
    /// doafter length when patching someone else's suit
    /// </summary>
    [DataField]
    public TimeSpan OtherApplyDelay = TimeSpan.FromSeconds(14);

    /// <summary>
    /// whether movement should cancel the patching process
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool BreakOnMove = true;
}
