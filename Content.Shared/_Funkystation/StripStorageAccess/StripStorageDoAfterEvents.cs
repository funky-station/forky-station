using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.StripStorageAccess;

// fired on the storage entity itself when opening it via the strip menu finishes
[Serializable, NetSerializable]
public sealed partial class StripStorageOpenDoAfterEvent : SimpleDoAfterEvent
{
}

// fired on the storage entity when a gated remove finishes or cancels
[Serializable, NetSerializable]
public sealed partial class StripStorageRemoveDoAfterEvent : SimpleDoAfterEvent
{
}
