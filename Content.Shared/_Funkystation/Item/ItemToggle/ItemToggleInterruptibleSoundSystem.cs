using Content.Shared._Funkystation.Item.ItemToggle.Components;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.Audio.Systems;

namespace Content.Shared._Funkystation.Item.ItemToggle;

/// <summary>
/// System for <see cref="ItemToggleInterruptibleSoundComponent"/>.
/// </summary>
/// <remarks>
/// If you want looping sounds, use <see cref="Content.Shared.Item.ItemToggle.Components.ItemToggleActiveSoundComponent"/> instead.
/// </remarks>
public sealed partial class ItemToggleInterruptibleSoundSystem : EntitySystem
{
    [Dependency] private SharedAudioSystem _audio = null!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ItemToggleInterruptibleSoundComponent, ItemToggledEvent>(OnItemToggle);
    }

    private void OnItemToggle(EntityUid uid, ItemToggleInterruptibleSoundComponent component, ref ItemToggledEvent args)
    {
        if (component.SoundActivate == null)
            return;

        if (args.Activated)
        {
            component.AudioStream = _audio.PlayPvs(component.SoundActivate, uid)?.Entity;
        }
        else
        {
            component.AudioStream = _audio.Stop(component.AudioStream);
        }
    }
}
