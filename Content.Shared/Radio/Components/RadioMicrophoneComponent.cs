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
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
[Access(typeof(SharedRadioDeviceSystem))]
public sealed partial class RadioMicrophoneComponent : Component
{
    [DataField]
    public ProtoId<RadioChannelPrototype> BroadcastChannel = SharedChatSystem.CommonChannel;

    // funky - change default listen range (and network it for ui)
    [DataField, AutoNetworkedField]
    public int ListenRange = 1;

    /// <summary>
    /// Funky - The maximum listen range
    /// this radio can have. This should
    /// be specified if the microphone
    /// sensitivity is adjustable (e.g.
    /// via UI.)
    /// </summary>
    [DataField]
    public int? MaxRange;
    /// <summary>
    /// Funky - The minimum listen range
    /// this radio can have, if adjustable
    /// by UI. Defaults to 1.
    /// </summary>
    [DataField]
    public int? MinRange = 1;

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
