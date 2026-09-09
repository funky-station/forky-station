using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Item.ItemToggle.Components;

/// <summary>
/// Handles additional sounds to play when an item is activated that can be interrupted when it is deactivated.
/// </summary>
/// <remarks>
/// If you want looping sounds, use <see cref="Content.Shared.Item.ItemToggle.Components.ItemToggleActiveSoundComponent"/> instead.
/// </remarks>
[RegisterComponent, NetworkedComponent]
public sealed partial class ItemToggleInterruptibleSoundComponent : Component
{
    /// <summary>
    /// An additional sound to play when the item is toggled on, which can be interrupted when it is toggled off.
    /// </summary>
    [DataField]
    public SoundSpecifier? SoundActivate;

    public EntityUid? AudioStream;
}
