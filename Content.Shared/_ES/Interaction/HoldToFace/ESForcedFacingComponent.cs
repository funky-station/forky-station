using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared._ES.Interaction.HoldToFace;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ESForcedFacingComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<EntityUid> Targets = new();

    [ViewVariables]
    public EntityUid? PrimaryTarget => Targets.FirstOrNull();
}

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ESForcedFacingTargetComponent : Component
{
    [DataField, AutoNetworkedField]
    public HashSet<EntityUid> Facing = new();
}
