using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.SuitBreach;

/// <summary>
/// raised after the doafter for applying a sealant canister to a breached suit
/// </summary>
[Serializable, NetSerializable]
public sealed partial class SuitSealDoAfterEvent : SimpleDoAfterEvent
{
}
