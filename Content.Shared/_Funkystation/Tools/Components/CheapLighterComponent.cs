using Robust.Shared.GameStates;
using Robust.Shared.Audio;

namespace Content.Shared._Funkystation.Tools.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class CheapLighterComponent : Component
{
    /// <summary>
    /// An additional sound the lighter should play when switched on, which can be interrupted when it's closed.
    /// </summary>
    [DataField]
    public SoundSpecifier? SoundActivate;

    /// <summary>
    /// The chance the lighter will fail to light, between 0.0 and 1.0.
    /// </summary>
    [DataField]
    public float FailChance = 0f;

    public EntityUid? AudioStream;
}
