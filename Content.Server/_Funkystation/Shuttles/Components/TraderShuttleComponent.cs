using Robust.Shared.Utility;

namespace Content.Server._Funkystation.Shuttles.Components;

[RegisterComponent]
public sealed partial class TraderShuttleComponent : Component
{
    /// <summary>
    ///  Size of the trader shuttle, 1 = small, 2 = medium, 3 = large
    /// </summary>
    [DataField("size", required: true)]
    public int Size { get; set; } = 2;

    /// <summary>
    /// Company that owns the trader shuttle, determines decals, items sold, etc.
    /// </summary>
    [DataField("company", required: true)]
    public string Company { get; set; } = "Nanotrasen";

    /// <summary>
    /// Path to the trader shuttle's map file, used for spawning the shuttle. Depends on size and company
    /// </summary>
    [DataField("shuttlePath")]
    public ResPath ShuttlePath { get; set; }
}
