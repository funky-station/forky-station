using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Cpr;

/// <summary>
/// Data for CPR animations
/// </summary>
[Serializable, NetSerializable]
public sealed class CprLungeEvent(NetEntity entity) : EntityEventArgs
{
    public NetEntity Ent = entity;
}
