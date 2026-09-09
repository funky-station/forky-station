using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Traits.Unions;

[RegisterComponent, NetworkedComponent]
public sealed partial class UnionLeaderComponent : Component
{
    public override bool SessionSpecific => true;
}
