using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.WarningTape;

/// <summary>
/// raised after doafter for tearing tape
/// </summary>
[Serializable, NetSerializable]
public sealed partial class TapeRemoveDoAfterEvent : SimpleDoAfterEvent
{
}
