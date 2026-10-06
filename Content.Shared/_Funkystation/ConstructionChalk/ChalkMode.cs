using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.ConstructionChalk;

[Serializable, NetSerializable]
public enum ChalkMode : byte
{
    Construction = 0,
    Piping = 1,
}
