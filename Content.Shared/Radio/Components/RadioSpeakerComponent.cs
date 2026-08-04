using Content.Shared.Actions;
using Content.Shared.Radio.EntitySystems;
using Content.Shared.Chat;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Radio.Components;

/// <summary>
///     Listens for radio messages and relays them to local chat.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
[Access(typeof(SharedRadioDeviceSystem))]
public sealed partial class RadioSpeakerComponent : Component
{
    /// <summary>
    /// Whether or not interacting with this entity
    /// toggles it on or off.
    /// </summary>
    [DataField]
    public bool ToggleOnInteract = true;

    [DataField]
    public HashSet<ProtoId<RadioChannelPrototype>> Channels = new() { SharedChatSystem.CommonChannel };

    [DataField, AutoNetworkedField]
    public bool Enabled;

    /// <summary>
    /// Funky - the length of the cooldown period
    /// for toggling the speaker
    /// </summary>
    /// <remarks>Intercom UI ignores this.</remarks>
    [DataField]
    public TimeSpan? Cooldown = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Funky - an action to add to the person holding
    /// the object, which toggles its speaker
    /// </summary>
    [DataField]
    public EntProtoId ActionId = "ActionToggleRadioSpeaker";

    [DataField]
    public EntityUid? ActionEntity; // funky addition

    /// <summary>
    /// Funky - the volume of the speaker.
    /// Messages are a whisper when volume
    /// is 2 or less.
    /// </summary>
    // if only we had more control over the range people can hear a message :(
    [DataField, AutoNetworkedField]
    public float Volume = 1;

    /// <summary>
    /// Funky - the max volume of the speaker.
    /// In most situations you should leave this unchanged.
    /// </summary>
    [DataField]
    public float MaxVolume = 4;
    /// <summary>
    /// Funky - the min volume of the speaker.
    /// In most situations you should leave this unchanged.
    /// </summary>
    [DataField]
    public float MinVolume = 1;
}
// Funky
public sealed partial class ToggleRadioSpeakerEvent : InstantActionEvent;
