using Robust.Shared.GameStates;
using Robust.Shared.Containers;

namespace Content.Shared._Funkystation.LaundryCart;

[RegisterComponent, NetworkedComponent]
public sealed partial class LaundryCartComponent : Component
{
    // how many bag slots need to be filled before someone can climb in
    [DataField]
    public int RequiredBags = 4;

    // max people that can cram in at once
    [DataField]
    public int MaxOccupants = 2;

    // container id for hidden mobs
    [DataField]
    public string HiddenContainerId = "hidden_mobs";

    // how long it takes to climb in
    [DataField]
    public float HideDelay = 2f;

    [ViewVariables]
    public Container HiddenContainer = null!;
}
