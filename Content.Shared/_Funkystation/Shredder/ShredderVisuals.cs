using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Shredder;

[Serializable, NetSerializable]
public enum ShredderVisuals : byte
{
    VisualState,
    Unshaded,
    BinPresent
}

[Serializable, NetSerializable]
public enum ShredderVisualState : byte
{
    Idle,
    Shredding
}
