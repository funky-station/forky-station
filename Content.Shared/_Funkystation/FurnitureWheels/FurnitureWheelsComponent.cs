using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.FurnitureWheels;

/// <summary>
/// Marks a piece of furniture as having lockable wheels. The wheels being "locked"
/// is equivalent to the entity being anchored — driving the state off
/// <see cref="TransformComponent.Anchored"/> means the verb, a wrench, and any
/// other anchor toggle path stay in sync automatically.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FurnitureWheelsComponent : Component
{
    [DataField]
    public SoundSpecifier? LockSound = new SoundPathSpecifier("/Audio/_Funkystation/Effects/wheelbrake.ogg")
    {
        Params = AudioParams.Default.WithVolume(-4f),
    };

    [DataField]
    public SoundSpecifier? UnlockSound = new SoundPathSpecifier("/Audio/_Funkystation/Effects/wheelbrake.ogg")
    {
        Params = AudioParams.Default.WithVolume(-4f).WithPitchScale(1.15f),
    };
}

[Serializable, NetSerializable]
public enum FurnitureWheelsVisuals : byte
{
    Locked
}
