using Content.Shared.DoAfter;
using Content.Shared.Tools;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.SerialNumber;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SerialNumberComponent : Component
{

    // <summary>
    //  Prefix before the random serial number
    // </summary>
    [DataField("prefix")]
    public string Prefix = "Property of NanoTrasen SN: NT-";

    // <summary>
    //  Suffix after the random serial number, just in case someone needs this in the future
    // </summary>
    [DataField("suffix")]
    public string Suffix = "";

    // <summary>
    //  Skip adding a random number between the prefix and suffix
    // </summary>
    [DataField("noNumber")]
    public bool NoNumber = false;

    // <summary>
    //  The actual serial number
    // </summary>
    [DataField("serialNumber"), AutoNetworkedField]
    public string SerialNumber = "123456";

    // <summary>
    //  The smallest the serial number can be.
    // </summary>
    [DataField("startNumber")]
    public int StartNumber = 250000;

    // <summary>
    //  The largest the serial number can be.
    // </summary>
    [DataField("endNumber")]
    public int EndNumber = 350000;

    // <summary>
    //  If the serial number can be removed
    // </summary>
    [DataField("canBeRemoved")]
    public bool CanBeRemoved = true;

    // <summary>
    //  If the serial number is scratched off or not
    // </summary>
    [DataField("removed"), AutoNetworkedField]
    public bool Removed = false;

    // <summary>
    //  Examine text if serial number is removed
    // </summary>
    [DataField("removedText")]
    public string RemovedText = "The serial number is removed";

    /// <summary>
    /// The tool quality needed to remove serial number
    /// </summary>
    [DataField("toolRequired")]
    public ProtoId<ToolQualityPrototype> ToolRequired = "Sawing";

    /// <summary>
    /// Amount of time in seconds it takes to remove the serial number
    /// </summary>
    [DataField("removalTime")]
    public TimeSpan RemovalTime = TimeSpan.FromSeconds(8f);

    /// <summary>
    /// Sound to make when removing the marking
    /// </summary>
    [DataField, ViewVariables]
    public SoundPathSpecifier RemoveSound = new ("/Audio/Items/Culinary/mortar_grinding.ogg");

    /// <summary>
    /// Stores doafter to prevent doafter spam/prediction issues
    /// </summary>
    [DataField, ViewVariables]
    public DoAfterId? SerialNumberDoAfter;
}

[Serializable, NetSerializable]
public sealed partial class SerialNumberDoAfterEvent : SimpleDoAfterEvent
{
}
