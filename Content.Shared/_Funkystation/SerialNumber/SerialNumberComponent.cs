namespace Content.Shared._Funkystation.SerialNumber;

[RegisterComponent]
public sealed partial class SerialNumberComponent : Component
{

    // <summary>
    //  This is what goes before the prefix. This variable in included in case you
    //  want to use this system for similar purposes that need a different leadup
    // </summary>
    [DataField("leadUp")] public string LeadUp = "Serial Number: ";

    // <summary>
    //  Prefix before serial number
    // </summary>
    [DataField("prefix")] public string Prefix = "NT-";

    // <summary>
    //  The serial number
    // </summary>
    [DataField("serialNumber")] public string SerialNumber = "123456";

    // <summary>
    //  Minimum amount of digits long the randomized number is.
    // </summary>
    [DataField("mindigits")] public int MinDigits = 6;

    // <summary>
    //  Minimum amount of digits long the randomized number is.
    // </summary>
    [DataField("maxdigits")] public int MaxDigits = 6;
}
