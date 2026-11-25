using Content.Shared.Radio;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.SM.Prototypes;

[Prototype]
public sealed partial class SupermatterAlertLevelPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("lowerThreshold")]
    public float LowerThreshold;

    [DataField("lowerState")]
    public ProtoId<SupermatterAlertLevelPrototype> LowerState;

    [DataField("upperThreshold")]
    public float UpperThreshold;

    [DataField("upperState")]
    public ProtoId<SupermatterAlertLevelPrototype> UpperState;

    [DataField("announceDecreaseToStation")]
    public bool AnnounceDecreaseToStation;

    [DataField("announceIncreaseToStation")]
    public bool AnnounceIncreaseToStation;

    [DataField("radioChannels")]
    public List<ProtoId<RadioChannelPrototype>> RadioChannels = [];

    [DataField("increasingRadioMessage")]
    public string IncreasingRadioMessage = string.Empty;

    [DataField("decreasingRadioMessage")]
    public string DecreasingRadioMessage = string.Empty;

    [DataField("increasingAnnouncementMessage")]
    public string IncreasingAnnouncementMessage = string.Empty;

    [DataField("decreasingAnnouncementMessage")]
    public string DecreasingAnnouncementMessage = string.Empty;
}
