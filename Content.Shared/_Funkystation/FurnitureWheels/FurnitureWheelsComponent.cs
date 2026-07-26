using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.FurnitureWheels;

[RegisterComponent, NetworkedComponent]
public sealed partial class FurnitureWheelsComponent : Component
{
    [DataField]
    public bool Locked = true;
}
