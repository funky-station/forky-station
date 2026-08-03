using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes; // Funky change
using Robust.Shared.Serialization;
using Content.Shared.Actions; // Funky change

namespace Content.Shared.SubFloor;

public sealed partial class ToggleTrayScannerEvent : InstantActionEvent
{
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TrayScannerComponent : Component
{
    /// <summary>
    ///     Whether the scanner is currently on.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Enabled;

    /// <summary>
    ///     Current mode of operation, defines which subfloor entities are shown.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TrayScannerMode Mode = TrayScannerMode.All;

    /// <summary>
    ///     Radius in which the scanner will reveal entities. Centered on the <see cref="LastLocation"/>.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Range = 4f;

    // Funky change
    /// <summary>
    ///     The action prototype to give to the user when equipped.
    /// </summary>
    [DataField]
    public EntProtoId? ToggleAction;

    // Funky change
    /// <summary>
    ///     The spawned action entity linked to this scanner.
    /// </summary>
    [DataField, NonSerialized]
    public EntityUid? ToggleActionEntity;

    // Funky change
    /// <summary>
    ///     Sound played when the scanner is turned on.
    /// </summary>
    [DataField]
    public SoundSpecifier? SoundOn;

    // Funky change
    /// <summary>
    ///     Sound played when the scanner is turned off.
    /// </summary>
    [DataField]
    public SoundSpecifier? SoundOff;

    [DataField]
    public SoundSpecifier SoundSwitchMode = new SoundPathSpecifier("/Audio/Machines/quickbeep.ogg");
}

// Funky change
[Serializable, NetSerializable]
public enum TrayScannerMode
{
    All,
    Piping,
    Wiring
}
