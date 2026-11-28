namespace Content.Shared._Funkystation.SM;

[DataDefinition]
[Serializable]
public sealed partial class LightningTier
{
    [DataField(required: true)]
    public float MinPower;

    [DataField(required: true)]
    public int BoltCount;

    [DataField(required: true)]
    public float BoltEnergyMJ;

    [DataField(required: true)]
    public int ExtraBounces;

    [DataField(required: true)]
    public string Proto = "Lightning";
}
