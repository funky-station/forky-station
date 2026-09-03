using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.WarningTape.Components;

/// <summary>
/// hand tool that places a tape line between two clicked points
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TapeRollComponent : Component
{
    // tape entity spawned per placement
    [DataField(required: true), AutoNetworkedField]
    public EntProtoId TapePrototype;

    [DataField, AutoNetworkedField]
    public float TileLength = 1f;

    // first click's location
    [DataField, AutoNetworkedField]
    public EntityCoordinates? PendingStart;

    [DataField, AutoNetworkedField]
    public EntityUid? PendingUser;

    [DataField]
    public SoundSpecifier SnapSound = new SoundPathSpecifier("/Audio/Effects/poster_broken.ogg");
}
