using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.SM.Prototypes;

[Prototype]
public sealed partial class GasCharacteristicsPrototype : IPrototype
{

    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("stability")]
    public float Stability { get; private set; }

    [DataField("growth")]
    public float Growth { get; private set; }

    [DataField("conductivity")]
    public float Conductivity { get; private set; }

    [DataField("enthalpy")]
    public float Enthalpy { get; private set; }
}
