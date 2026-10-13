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
    //  The serial number
    // </summary>
    [DataField("serialNumber"), AutoNetworkedField]
    public string SerialNumber = "123456";

    // <summary>
    //  The smallest the number the serial number can be.
    // </summary>
    [DataField("mindigits")]
    public int MinDigit = 250000;

    // <summary>
    //  The biggest the number the serial number can be.
    // </summary>
    [DataField("maxdigits")]
    public int MaxDigit = 350000;

    // <summary>
    //  If the serial number is scratched off or not
    // </summary>
    [DataField("scratchedOff"), AutoNetworkedField]
    public bool ScratchedOff = false;

    /// <summary>
    /// The tool quality needed to remove serial number
    /// </summary>
    [DataField("toolRequired")]
    public ProtoId<ToolQualityPrototype> ToolRequired = "Sawing";

    /// <summary>
    /// Amount of time in seconds it takes to remove the serial number
    /// </summary>
    [DataField("scratchingTime")]
    public TimeSpan ScratchingTime = TimeSpan.FromSeconds(4f);

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
