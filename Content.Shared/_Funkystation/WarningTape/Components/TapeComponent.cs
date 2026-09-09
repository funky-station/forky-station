using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.WarningTape.Components;

/// <summary>
/// lets you tear tape with an empty hand or tool doafter
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class TapeComponent : Component
{
    /// <summary>
    /// how long tearing the tape down with empty hands takes
    /// </summary>
    [DataField]
    public TimeSpan RemoveDelay = TimeSpan.FromSeconds(1.5);

    /// <summary>
    /// vs with cutting tools
    /// </summary>
    [DataField]
    public TimeSpan CutDelay = TimeSpan.FromSeconds(0.5);

    [DataField]
    public SoundSpecifier RemoveSound = new SoundPathSpecifier("/Audio/Effects/poster_broken.ogg");
}
