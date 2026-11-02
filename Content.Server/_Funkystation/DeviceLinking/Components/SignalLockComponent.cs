using Content.Shared.DeviceLinking;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._Funkystation.DeviceLinking.Components;

[RegisterComponent]
public sealed partial class SignalLockComponent : Component
{
    /// <summary>
    /// The type of key that activates the lock.
    /// This needs to be the same value as the KeyType on the key's SignalKey component.
    /// </summary>
    [DataField]
    public string? KeyType;

    /// <summary>
    /// The port that gets invoked when used.
    /// </summary>
    [DataField]
    public ProtoId<SourcePortPrototype> Port = "Trigger";

    [DataField]
    public SoundSpecifier LockSound = new SoundPathSpecifier("/Audio/_Funkystation/Machines/signal_lock.ogg");

    [DataField]
    public SoundSpecifier WrongKeySound = new SoundPathSpecifier("/Audio/_Funkystation/Machines/signal_lock_wrong_key.ogg");
}
