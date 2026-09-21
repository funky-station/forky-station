using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Shared._Funkystation.WarningTape.Components;

/// <summary>
/// client overlay that draws a tiled tape texture
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(raiseAfterAutoHandleState: true)]
public sealed partial class TapeVisualsComponent : Component
{
    /// <summary>
    /// the tape texture. drawn once per tile repeat along the line
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public SpriteSpecifier Sprite = null!;

    /// <summary>
    /// pixel sub-region of Sprite that's actually the tape
    /// </summary>
    [DataField, AutoNetworkedField]
    public Box2 ContentRegion = new(0, 13, 32, 19);

    /// <summary>
    /// the far end of the line
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityCoordinates End;

    /// <summary>
    /// number of texture repeats between start and end
    /// </summary>
    [DataField, AutoNetworkedField]
    public int TileCount = 1;
}
