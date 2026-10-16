using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Mobs;

[Prototype]
public sealed partial class MobSizePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("smPower")]
    public float SmPower;
}
