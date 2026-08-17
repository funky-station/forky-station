
namespace Content.Shared._Funkystation.TraderCargo.Components;

[RegisterComponent]
public sealed partial class FTLCommsComputerComponent : Component
{
    /// <summary>
    /// The first trader shuttle in the FTL comms menu
    /// </summary>
    [DataField("traderShuttle")]
    public EntityUid? TraderShuttle1 { get; set; }
}
