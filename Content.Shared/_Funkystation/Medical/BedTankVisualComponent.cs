using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Medical;

[RegisterComponent, NetworkedComponent]
public sealed partial class BedTankVisualComponent : Component
{
    [DataField(required: true)]
    public BedTankVisual Visual;
}
