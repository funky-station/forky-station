using Content.Shared.Actions;
using Content.Shared.Radio.EntitySystems;
using Content.Shared.Chat;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Radio.Components;

/// <summary>
///     Listens for local chat messages and relays them to some radio frequency
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedRadioDeviceSystem))]
public sealed partial class RadioMicrophoneComponent : Component
{
    [DataField]
    public ProtoId<RadioChannelPrototype> BroadcastChannel = SharedChatSystem.CommonChannel;

    [DataField]
    public int ListenRange = 4;

    [DataField]
    public bool Enabled = false;

    [DataField]
    public bool PowerRequired = false;

    /// <summary>
    /// Whether or not interacting with this entity
    /// toggles it on or off.
    /// </summary>
    [DataField]
    public bool ToggleOnInteract = true;

    /// <summary>
    /// Whether or not the speaker must have an
    /// unobstructed path to the radio to speak
    /// </summary>
    [DataField]
    public bool UnobstructedRequired = false;

    [DataField]
    public TimeSpan? Cooldown = TimeSpan.FromSeconds(1); // funky addition

    [DataField]
    public bool SpeakerRequired = true; // funky addition

    [DataField]
    public SoundSpecifier? ToggleOnSound; // funky addition
    [DataField]
    public SoundSpecifier? ToggleOffSound; // funky addition

    [DataField]
    public EntProtoId ActionId = "ActionToggleRadioMicrophone"; // funky addition

    [DataField]
    public EntityUid? ActionEntity; // funky addition

}

public sealed partial class ToggleRadioMicrophoneEvent : InstantActionEvent; // funky addition
