using Robust.Shared.Audio; // Funky change
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes; // Funky change
using Robust.Shared.Serialization;
using Content.Shared.Actions; // Funky change

namespace Content.Shared.SubFloor;

public sealed partial class ToggleTrayScannerEvent : InstantActionEvent
{
}

[RegisterComponent, NetworkedComponent]
public sealed partial class TrayScannerComponent : Component
{
    /// <summary>
    ///     Whether the scanner is currently on.
    /// </summary>
    [DataField]
    public bool Enabled;

    /// <summary>
    ///     Radius in which the scanner will reveal entities. Centered on the <see cref="LastLocation"/>.
    /// </summary>
    [DataField]
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
}

// Funky change
[Serializable, NetSerializable]
public sealed class TrayScannerState(bool enabled, float range) : ComponentState
{
    public bool Enabled = enabled;
    public float Range = range;
}
