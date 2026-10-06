using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.LaundryCart;

[Serializable, NetSerializable]
public sealed partial class LaundryCartHideDoAfterEvent : SimpleDoAfterEvent
{
}
