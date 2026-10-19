using Content.Shared.DoAfter;
using Content.Shared.Tools;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.SerialNumber;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SerialNumberComponent : Component
{

    // <summary>
    //  This is what goes before the prefix. This variable in included in case you
    //  want to use this system for similar purposes that need a different leadup
    // </summary>
    [DataField("leadUp")]
    public string LeadUp = "Serial Number: ";

    // <summary>
    //  Prefix before serial number
    // </summary>
    [DataField("prefix")]
    public string Prefix = "NT-";

    // <summary>
    //  Skip adding a random number to the end
    // </summary>
    [DataField("noNumber")]
    public bool NoNumber = false;

    // <summary>
    //  The serial number
    // </summary>
    [DataField("serialNumber"), AutoNetworkedField]
    public string SerialNumber = "123456";

    // <summary>
    //  The smallest the serial number can be.
    // </summary>
    [DataField("startNumber")]
    public int StartNumber = 250000;

    // <summary>
    //  The biggest the serial number can be.
    // </summary>
    [DataField("endNumber")]
    public int EndNumber = 350000;

    // <summary>
    //  If the serial number is scratched off or not
    // </summary>
    [DataField("removed"), AutoNetworkedField]
    public bool Removed = false;

    // <summary>
    //  If the serial number can be removed
    // </summary>
    [DataField("canBeRemoved")]
    public bool CanBeRemoved = true;

    // <summary>
    //  Examine text if serial number is removed
    // </summary>
    [DataField("removedText")]
    public string RemovedText = "The serial number is scratched off";

    /// <summary>
    /// The tool quality needed to remove serial number
    /// </summary>
    [DataField("toolRequired")]
    public ProtoId<ToolQualityPrototype> ToolRequired = "Sawing";

    /// <summary>
    /// Amount of time in seconds it takes to remove the serial number
    /// </summary>
    [DataField("removalTime")]
    public TimeSpan RemovalTime = TimeSpan.FromSeconds(4f);

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
