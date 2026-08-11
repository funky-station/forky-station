
namespace Content.Server._Funkystation.Shuttles.Components;

/// <summary>
/// Added to a station that is available for trader shuttles.
/// </summary>

[RegisterComponent]
public sealed partial class StationTraderComponent : Component
{
    [DataField("traderArrivalMinOffset")]
    public float TraderArrivalMinOffset = 96f;

    [DataField("traderArrivalMaxOffset")]
    public float TraderArrivalMaxOffset = 160f;
    /// <summary>
    /// Maximum number of trader shuttles that can be called to this station at once. Not counting the delivery shuttle
    /// </summary>
    [DataField("maxTraders")]
    public int MaxTraders = 3;

    /// <summary>
    /// Current number of trader shuttles at the station.
    /// </summary>
    [DataField("currentTraders")]
    public int CurrentTraders = 0;
}
