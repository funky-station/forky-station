using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Handwriting;

/// <summary>
/// given by handwriting trait
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HandwritingFontComponent : Component
{
    // id of a FontPrototype
    [DataField("fontId"), AutoNetworkedField]
    public string FontId = "HandwritingCasual";

    // size to render the font at
    [DataField("fontSize"), AutoNetworkedField]
    public int FontSize = 20;
}
