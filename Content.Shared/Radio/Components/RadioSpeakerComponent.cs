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

    [DataField]
    public TimeSpan? Cooldown = TimeSpan.FromSeconds(1); // funky addition

    [DataField]
    public EntProtoId ActionId = "ActionToggleRadioSpeaker"; // funky addition

    [DataField]
    public EntityUid? ActionEntity; // funky addition

    [DataField, AutoNetworkedField]
    public int Volume = 1; // funky
}

public sealed partial class ToggleRadioSpeakerEvent : InstantActionEvent;
