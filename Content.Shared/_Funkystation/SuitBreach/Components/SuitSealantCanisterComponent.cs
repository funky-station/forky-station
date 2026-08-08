using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.SuitBreach.Components;

/// <summary>
/// suit sealant foam canister
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SuitSealantCanisterComponent : Component
{
    [DataField, AutoNetworkedField]
    public int Charges = 2;

    /// <summary>
    /// doafter length when patching your own suit
    /// </summary>
    [DataField]
    public TimeSpan SelfApplyDelay = TimeSpan.FromSeconds(4);

    /// <summary>
    /// doafter length when patching someone else's suit
    /// </summary>
    [DataField]
    public TimeSpan OtherApplyDelay = TimeSpan.FromSeconds(2);

    /// <summary>
    /// sound played when successfully patching a suit
    /// </summary>
    [DataField]
    public SoundSpecifier ApplySound = new SoundPathSpecifier("/Audio/Effects/spray.ogg");
}
