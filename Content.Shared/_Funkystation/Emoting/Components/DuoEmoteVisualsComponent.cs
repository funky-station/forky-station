using Content.Shared._Funkystation.Emoting.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Emoting.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class DuoEmoteVisualsComponent : Component
{
    [DataField, AutoNetworkedField]
    public ProtoId<DuoEmotePrototype> EmoteId;
}
