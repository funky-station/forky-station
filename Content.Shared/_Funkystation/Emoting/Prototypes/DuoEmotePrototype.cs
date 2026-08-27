using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._Funkystation.Emoting.Prototypes;

[Prototype]
public sealed partial class DuoEmotePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = null!;

    [DataField(required: true)]
    public string Name { get; private set; } = null!;

    [DataField(required: true)]
    public SpriteSpecifier Icon { get; private set; } = null!;

    [DataField(required: true)]
    public SpriteSpecifier EffectSprite { get; private set; } = null!;

    [DataField]
    public SoundSpecifier? Sound { get; private set; }

    [DataField(required: true)]
    public LocId AttemptSelf { get; private set; }

    [DataField(required: true)]
    public LocId AttemptOthers { get; private set; }

    [DataField(required: true)]
    public LocId PerformSelf { get; private set; }

    [DataField(required: true)]
    public LocId PerformOthers { get; private set; }

    [DataField]
    public bool RequireTail { get; private set; }

    [DataField]
    public string? Animation { get; private set; }
}
