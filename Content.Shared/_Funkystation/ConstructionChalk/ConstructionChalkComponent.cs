using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.ConstructionChalk;

// tool that places construction chalk marks
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ConstructionChalkComponent : Component
{
    // which set of categories the radial shows
    [DataField, AutoNetworkedField]
    public ChalkMode Mode = ChalkMode.Construction;

    [DataField]
    public SoundSpecifier PlaceSound = new SoundCollectionSpecifier("Chalk");
}
