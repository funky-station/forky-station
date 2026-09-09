using Content.Shared._Funkystation.Emoting.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Emoting;

/// <summary>
/// Sent by the client when selecting a duo emote from the radial
/// </summary>
[Serializable, NetSerializable]
public sealed class DuoEmoteAttemptEvent(NetEntity target, ProtoId<DuoEmotePrototype> emoteId) : EntityEventArgs
{
    public NetEntity Target { get; } = target;
    public ProtoId<DuoEmotePrototype> EmoteId { get; } = emoteId;
}
