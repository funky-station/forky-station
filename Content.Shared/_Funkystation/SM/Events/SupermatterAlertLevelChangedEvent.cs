using Content.Shared._Funkystation.SM.Prototypes;

namespace Content.Shared._Funkystation.SM.Events;

[ByRefEvent]
public readonly record struct SupermatterAlertLevelChangedEvent(SupermatterAlertLevelPrototype oldLevel, SupermatterAlertLevelPrototype newLevel, bool stabilityIncreasing)
{
    public readonly SupermatterAlertLevelPrototype OldLevel = oldLevel;
    public readonly SupermatterAlertLevelPrototype NewLevel = newLevel;
    public readonly bool StabilityIncreasing = stabilityIncreasing;
}
