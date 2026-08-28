using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.SuitBreach.Components;

/// <summary>
/// added to a suit while it's breached
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class SuitBreachedComponent : Component
{
    [DataField, AutoNetworkedField]
    public SuitBreachSeverity Severity = SuitBreachSeverity.Minor;

    /// <summary>
    /// tracks if there's a tank connected and not empty
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IsLeaking;

    /// <summary>
    /// "angle" of the escaping gas jet, for pushing you around
    /// </summary>
    [DataField, AutoNetworkedField]
    public Angle LeakAngle;

    /// <summary>
    /// how many moles/sec drawn from the connected tank per severity tier
    /// tuned so a standard 5L 1000kpa tank, which is 2 moles, empties in roughly 40s/10s/4s
    /// </summary>
    [DataField]
    public Dictionary<SuitBreachSeverity, float> DrainRatesPerSecond = new()
    {
        { SuitBreachSeverity.Minor, 0.05f },
        { SuitBreachSeverity.Major, 0.2f },
        { SuitBreachSeverity.Catastrophic, 0.5f },
    };

    /// <summary>
    /// force of the push applied to a weightless wearer while leaking gas
    /// </summary>
    [DataField]
    public Dictionary<SuitBreachSeverity, float> LeakImpulsePerSecond = new()
    {
        { SuitBreachSeverity.Minor, 15f },
        { SuitBreachSeverity.Major, 45f },
        { SuitBreachSeverity.Catastrophic, 100f },
    };

    /// <summary>
    /// volume adjustment per severity for the hiss sound
    /// </summary>
    [DataField]
    public Dictionary<SuitBreachSeverity, float> HissVolumePerSeverity = new()
    {
        { SuitBreachSeverity.Minor, -8f },
        { SuitBreachSeverity.Major, -3f },
        { SuitBreachSeverity.Catastrophic, 2f },
    };

    /// <summary>
    /// ambient hiss heard by nearby players
    /// </summary>
    [DataField]
    public SoundSpecifier HissSound = new SoundPathSpecifier("/Audio/_Funkystation/Effects/suit_breach_hiss.ogg");

    /// <summary>
    /// currently-playing hiss stream
    /// </summary>
    [ViewVariables]
    public EntityUid? HissStream;
}
