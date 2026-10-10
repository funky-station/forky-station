using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Mobs;
[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(MobSizeSystem))]
public sealed partial class MobSizeComponent: Component
{

    [DataField("size")]
    [AutoNetworkedField, ViewVariables]
    public string SizeId = "Medium";

    [ViewVariables]
    public MobSizePrototype? SizeProto;
}
