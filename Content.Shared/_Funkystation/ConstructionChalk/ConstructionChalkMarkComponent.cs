using Content.Shared.Construction.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.ConstructionChalk;

// a placed chalk mark
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class ConstructionChalkMarkComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<ConstructionPrototype>? ConstructionPrototype;

    [DataField, AutoNetworkedField]
    public Angle Rotation = Angle.Zero;

    [DataField]
    public SoundSpecifier EraseSound = new SoundPathSpecifier("/Audio/Items/wirebrushing.ogg");

    [ViewVariables]
    public bool IsBuilding = false;
}
