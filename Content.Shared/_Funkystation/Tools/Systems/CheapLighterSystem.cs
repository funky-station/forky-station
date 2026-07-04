using Content.Shared._Funkystation.Tools.Components;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;

namespace Content.Shared._Funkystation.Tools.Systems;

public sealed partial class CheapLighterSystem : EntitySystem
{
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private IRobustRandom _random = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CheapLighterComponent, ItemToggledEvent>(OnLighterToggle);
        SubscribeLocalEvent<CheapLighterComponent, ItemToggleActivateAttemptEvent>(OnToggleOnAttempt);
    }

    private void OnToggleOnAttempt(EntityUid uid, CheapLighterComponent component, ref ItemToggleActivateAttemptEvent args)
    {
        if (_random.NextFloat() < component.FailChance)
        {
            args.Cancelled = true;
            // TODO: Popup text when the lighter fails to light
        }
    }
    private void OnLighterToggle(EntityUid uid, CheapLighterComponent component, ref ItemToggledEvent args)
    {
        if (args.Activated && component.SoundActivate != null)
        {
            component.AudioStream = _audio.PlayPvs(component.SoundActivate, uid)?.Entity;
        }
        else
        {
            _audio.Stop(component.AudioStream);
        }
    }
}
