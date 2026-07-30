using Content.Shared._Funkystation.Emoting.EntitySystems;
using Content.Shared._Funkystation.Emoting.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Funkystation.Emoting.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
[Access(typeof(SharedDuoEmoteSystem))]
public sealed partial class DuoEmoteComponent : Component
{
    /// <summary>
    /// Whether this entity is currently waiting for a partner to complete a duo emote
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Active;

    /// <summary>
    /// The entity that initiated the emote is waiting on
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Target;

    /// <summary>
    /// The visual effect entity spawned while waiting
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? SpawnedEffect;

    /// <summary>
    /// Which emote state is being attempted
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<DuoEmotePrototype>? EmoteId;

    /// <summary>
    /// How long the initiator can wait before being left hanging. Ten is very generous.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan LeftHangingDelay = TimeSpan.FromSeconds(10);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField, AutoPausedField]
    public TimeSpan LeaveHangingAt;

    /// <summary>
    /// Range within which a partner must be to complete the emote
    /// </summary>
    [DataField]
    public float InteractRange = 1.5f;
}
