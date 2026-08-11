using Robust.Shared.Utility;

namespace Content.Server._Funkystation.Shuttles.Components;

[RegisterComponent]
public sealed partial class TraderShuttleComponent : Component
{
    /// <summary>
    /// UID of the shuttle
    /// </summary>
    [DataField("shuttle")]
    public EntityUid Shuttle;

    /// <summary>
    /// UID of the station that called this trader shuttle
    /// </summary>
    [DataField("station")]
    public EntityUid Station;

    /// <summary>
    /// Path to the trader shuttle's map file, used for spawning the shuttle. Depends on size and company
    /// </summary>
    [DataField("shuttlePath")]
    public ResPath ShuttlePath { get; set; }
}
