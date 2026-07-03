using Content.Shared._Funkystation.Tools.Components;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;

namespace Content.Server._Funkystation.Tools.Systems;

public sealed partial class CheapLighterSystem : EntitySystem
{
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private IRobustRandom _random = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CheapLighterComponent, ItemToggleDeactivateAttemptEvent>(OnLighterToggleOff);
        SubscribeLocalEvent<CheapLighterComponent, ItemToggleActivateAttemptEvent>(OnLighterToggleOn);
    }

    private void OnLighterToggleOff(EntityUid uid, CheapLighterComponent component, ref ItemToggleDeactivateAttemptEvent args)
    {
        if (!args.Cancelled)
        {
            _audio.Stop(component.AudioStream);
        }
    }

    private void OnLighterToggleOn(EntityUid uid, CheapLighterComponent component, ref ItemToggleActivateAttemptEvent args)
    {
        if (_random.NextFloat() < component.FailChance)
        {
            args.Cancelled = true;
            _audio.PlayPvs(component.SoundFail, uid);
            // TODO: Popup text when the lighter fails to light
            return;
        }

        if (!args.Cancelled && component.SoundActivate != null)
        {
            component.AudioStream = _audio.PlayPvs(component.SoundActivate, uid)?.Entity;
        }
    }
}
