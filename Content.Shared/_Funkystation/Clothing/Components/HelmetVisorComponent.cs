using Content.Shared.Actions;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Clothing.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HelmetVisorComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool IsActive;

    [DataField]
    public EntProtoId Action = "ActionToggleHelmetVisor";

    [DataField]
    public EntityUid? ActionEntity;

    [DataField]
    public string StateUp = "";

    [DataField]
    public string StateDown = "equipped-head-up";

    [DataField]
    public string VisualLayer = "visor";

    [DataField]
    public SoundSpecifier? ToggleSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg");

    [DataField]
    public ComponentRegistry Components = new();
}

public sealed partial class ToggleHelmetVisorEvent : InstantActionEvent
{
}

[Serializable, NetSerializable]
public enum HelmetVisorVisuals : byte
{
    IsDown
}
