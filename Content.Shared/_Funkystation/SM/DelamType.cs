using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.SM;

/// <summary>
/// Which catastrophic outcome runs when the crystal delaminates.
/// </summary>
[Serializable, NetSerializable]
public enum DelamType : byte
{
    Singulo = 0,
    Tesla = 1,
    Explosion = 2,
    Cascade = 3,
}
