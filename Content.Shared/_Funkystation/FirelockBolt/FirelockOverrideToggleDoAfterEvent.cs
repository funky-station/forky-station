using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.FirelockBolt;

[Serializable, NetSerializable]
public sealed partial class FirelockOverrideToggleDoAfterEvent : DoAfterEvent
{
    [DataField]
    public bool TargetOverride;

    private FirelockOverrideToggleDoAfterEvent()
    {
    }

    public FirelockOverrideToggleDoAfterEvent(bool targetOverride)
    {
        TargetOverride = targetOverride;
    }

    public override DoAfterEvent Clone()
    {
        return this;
    }
}
