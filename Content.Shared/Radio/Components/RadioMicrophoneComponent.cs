using Content.Shared.Actions;
using Content.Shared.Radio.EntitySystems;
using Content.Shared.Chat;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Radio.Components;

/// <summary>
/// Listens for local chat messages and relays them to some radio frequency.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
[Access(typeof(SharedRadioDeviceSystem))]
public sealed partial class RadioMicrophoneComponent : Component
{
    /// <summary>
    /// Radio channel on which local speech is broadcast.
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<RadioChannelPrototype> BroadcastChannel = SharedChatSystem.CommonChannel;

    /// <summary>
    /// Maximum distance from the microphone at which speech is heard.
    /// </summary>
    [DataField, AutoNetworkedField] // funky - network listen range for ui
    public int ListenRange = 4;

    /// <summary>
    /// Funky - The maximum listen range this radio
    /// can have. This should be specified if the
    /// microphone sensitivity is adjustable (e.g.
    /// via UI.)
    /// </summary>
    [DataField]
    public int? MaxRange;
    /// <summary>
    /// Funky - The minimum listen range this radio
    /// can have, if adjustable by UI. Defaults to 1.
    /// </summary>
    [DataField]
    public int? MinRange = 1;

    /// <summary>
    /// Whether the microphone is currently broadcasting.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Enabled;

    /// <summary>
    /// Whether the microphone requires power to operate.
    /// </summary>
    [DataField]
    public bool PowerRequired;

    /// <summary>
    /// Whether interacting with this entity toggles it on/off, or not.
    /// </summary>
    [DataField]
    public bool ToggleOnInteract = true;

    /// <summary>
    /// Whether the speaker must have an unobstructed path to the radio to speak, or now.
    /// </summary>
    [DataField]
    public bool UnobstructedRequired;

    /// <summary>
    /// Funky - the length of the cooldown period
    /// for toggling the microphone on and off
    /// </summary>
    /// <remarks>Intercom UI ignores this.</remarks>
    [DataField]
    public TimeSpan? Cooldown = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Funky - whether or not the object this mic
    /// is attached to must have a speaker that is
    /// switched on
    /// </summary>
    /// <remarks>Intercom UI ignores this.</remarks>
    [DataField]
    public bool SpeakerRequired = false;

    /// <summary>
    /// Funky - a sound to play when this mic is
    /// toggled on
    /// </summary>
    /// <remarks>Intercom UI ignores this.</remarks>
    [DataField]
    public SoundSpecifier? ToggleOnSound;
    /// <summary>
    /// Funky - a sound to play when this mic is
    /// toggled off
    /// </summary>
    /// <remarks>Intercom UI ignores this.</remarks>
    [DataField]
    public SoundSpecifier? ToggleOffSound; // funky addition

    /// <summary>
    /// Funky - an action to add to the person holding
    /// the object, which toggles its mic
    /// </summary>
    [DataField]
    public EntProtoId ActionId = "ActionToggleRadioMicrophone"; // funky addition

    [DataField]
    public EntityUid? ActionEntity; // funky addition
}
// Funky
public sealed partial class ToggleRadioMicrophoneEvent : InstantActionEvent;
