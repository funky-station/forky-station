using Robust.Shared.GameStates;

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
    public int MinDigit = 100000;

    // <summary>
    //  The biggest the number the serial number can be.
    // </summary>
    [DataField("maxdigits")]
    public int MaxDigit = 999999;
}
