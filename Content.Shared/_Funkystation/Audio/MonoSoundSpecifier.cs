using Robust.Shared.Audio;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Audio;

/// <summary>
/// Specifies a pair of <see cref="SoundSpecifier"/>s,
/// one in stereo, one in mono.
/// </summary>
/// <why> Because we don't have stereo sound spatialization,
/// this is for specifying an accompanying mono sound for
/// certain sounds which may be played both in-world
/// and out-of-world for which we want to keep the
/// stereo version (because it sounds nice, for instance).
/// </why>
[DataDefinition, Serializable, NetSerializable]
public sealed partial class MonoSoundSpecifier
{
    [DataField(required: true)]
    public SoundSpecifier StereoSound { get; set; }

    [DataField(required: true)]
    public SoundSpecifier MonoSound { get; set; }
}
