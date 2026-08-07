using Content.Shared.Actions;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Radio.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared.Radio.EntitySystems;

public abstract partial class SharedRadioDeviceSystem : EntitySystem
{
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private SharedActionsSystem _actions = null!; // Funky - add speaker/mic toggle actions
    [Dependency] private UseDelaySystem _delays = null!; // Funky - toggle cooldown
    [Dependency] private SharedPowerReceiverSystem _powerReceiverSystem = null!; // funky - speaker power states


    public override void Initialize() // funky additions!!
    {
        base.Initialize();

        // BEGIN Funkystation
        SubscribeLocalEvent<RadioSpeakerComponent, GetVerbsEvent<AlternativeVerb>>(AddSpeakerToggleVerb);
        SubscribeLocalEvent<RadioSpeakerComponent, ToggleRadioSpeakerEvent>(OnSpeakerToggleAction);
        SubscribeLocalEvent<RadioSpeakerComponent, GetItemActionsEvent>(OnGetActions);
        SubscribeLocalEvent<RadioSpeakerComponent, ActivateInWorldEvent>(OnActivateSpeaker);

        SubscribeLocalEvent<RadioMicrophoneComponent, ActivateInWorldEvent>(OnActivateMicrophone);
        SubscribeLocalEvent<RadioMicrophoneComponent, GetVerbsEvent<AlternativeVerb>>(AddMicToggleVerb);
        SubscribeLocalEvent<RadioMicrophoneComponent, ToggleRadioMicrophoneEvent>(OnMicrophoneToggleAction);
        SubscribeLocalEvent<RadioMicrophoneComponent, GetItemActionsEvent>(OnGetActions);
        // END Funkystation
    }

    // this entire region is funky additions
    #region Funky - Verbs and actions
    private void AddSpeakerToggleVerb(EntityUid uid, RadioSpeakerComponent component, GetVerbsEvent<AlternativeVerb> args) // Funkystation
    {
        if(!args.CanAccess || !args.CanInteract)
            return;

        if (!component.ToggleOnInteract)
            return;

        AlternativeVerb verb = new()
        {
            Act = () =>
            {
                ToggleRadioSpeaker(uid, args.User, false, component);
            },
            Text = Loc.GetString("handheld-radio-component-power-verb"),
            Priority = 1,
            Icon = new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/Spare/poweronoff.svg.192dpi.png")),
            Message =  Loc.GetString("handheld-radio-component-speaker-desc"),
        };
        args.Verbs.Add(verb);
    }

    private void AddMicToggleVerb(EntityUid uid, RadioMicrophoneComponent component, GetVerbsEvent<AlternativeVerb> args) // Funkystation
    {
        if(!args.CanAccess || !args.CanInteract)
            return;

        if (!component.ToggleOnInteract)
            return;

        var disabled = false;
        var message = Loc.GetString("handheld-radio-component-mic-desc");
        if (component.SpeakerRequired && (!TryComp<RadioSpeakerComponent>(uid, out var speaker) || !speaker.Enabled))
        {
            disabled = true;
            message = Loc.GetString("handheld-radio-component-mic-desc-disabled");
        }

        AlternativeVerb verb = new()
        {
            Act = () =>
            {
                ToggleRadioMicrophone(uid, args.User, false, component);
            },
            Text = Loc.GetString("handheld-radio-component-mic-verb"),
            Priority = 0,
            Icon = new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/signal.svg.192dpi.png")),
            Disabled = disabled,
            Message = message,
        };
        args.Verbs.Add(verb);
    }

    private void OnSpeakerToggleAction(EntityUid uid, RadioSpeakerComponent component, ToggleRadioSpeakerEvent args)
    {
        if (args.Handled)
            return;

        if (!component.ToggleOnInteract)
            return;

        ToggleRadioSpeaker(uid, args.Performer, false, component);
        args.Handled = true;
    }

    private void OnMicrophoneToggleAction(EntityUid uid, RadioMicrophoneComponent component, ToggleRadioMicrophoneEvent args)
    {
        if (args.Handled)
            return;

        if (!component.ToggleOnInteract)
            return;

        if (component.SpeakerRequired && (!TryComp<RadioSpeakerComponent>(uid, out var speaker) || !speaker.Enabled))
        {
            _popup.PopupClient(Loc.GetString("handheld-radio-component-mic-desc-disabled"), uid, args.Performer);
            return;
        }

        ToggleRadioMicrophone(uid, args.Performer, false, component);
        args.Handled = true;
    }

    private void OnGetActions(EntityUid uid, RadioSpeakerComponent component, GetItemActionsEvent args)
    {
        _actions.AddAction(args.User, ref component.ActionEntity, component.ActionId, uid);
        if (component.Cooldown.HasValue)
            _actions.SetUseDelay(component.ActionEntity, component.Cooldown.Value); // overrides whatever cooldown the base action has
    }
    private void OnGetActions(EntityUid uid, RadioMicrophoneComponent component, GetItemActionsEvent args)
    {
        _actions.AddAction(args.User, ref component.ActionEntity, component.ActionId, uid);
        if (component.Cooldown.HasValue)
            _actions.SetUseDelay(component.ActionEntity, component.Cooldown.Value); // overrides whatever cooldown the base action has
    }
    #endregion
    #region Toggling
    // funky - toggling the radio as a whole (the speaker) with Z if the speaker is off or there is no mic present (otherwise, toggle the mic)
    private void OnActivateSpeaker(EntityUid uid, RadioSpeakerComponent speaker, ActivateInWorldEvent args)
    {
        if (!args.Complex)
            return;

        if (!speaker.ToggleOnInteract)
            return;

        if (speaker.Enabled && HasComp<RadioMicrophoneComponent>(uid))
            return;

        ToggleRadioSpeaker(uid, args.User, args.Handled, speaker);

        args.Handled = true;
    }
    // funky - toggling the microphone with Z if the speaker is turned on
    private void OnActivateMicrophone(EntityUid uid, RadioMicrophoneComponent mic, ActivateInWorldEvent args)
    {
        if (args.Handled) // we dont want the mic to turn on at the same time as the speaker (does this actually ensure that?)
            return;

        if (!args.Complex)
            return;

        if (!mic.ToggleOnInteract)
            return;
        // fail if this radio requires that the speaker be switched on and there is no speaker / its switched off
        if (mic.SpeakerRequired && (!TryComp<RadioSpeakerComponent>(uid, out var speaker) || !speaker.Enabled))
            return;

        ToggleRadioMicrophone(uid, args.User, args.Handled, mic);

        args.Handled = true;
    }
    public void ToggleRadioMicrophone(EntityUid uid, EntityUid user, bool quiet = false, RadioMicrophoneComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        // no matter how the component is toggled, we want the cooldown to be enforced (funky station)
        if (TryComp<UseDelayComponent>(uid, out var delayComp) && component.Cooldown.HasValue)
        {
            if (_delays.IsDelayed((uid, delayComp)))
                return;

            _delays.SetLength((uid, delayComp), component.Cooldown.Value);
            _delays.TryResetDelay((uid, delayComp));

        }

        _actions.SetToggled(component.ActionEntity, !component.Enabled); // funky
        SetMicrophoneEnabled(uid, user, !component.Enabled, quiet, component);
    }

    // seems to be this way because it can't be predicted on client? https://github.com/space-wizards/space-station-14/pull/39484#discussion_r2263840164
    public virtual void SetMicrophoneEnabled(EntityUid uid, EntityUid? user, bool enabled, bool quiet = false, RadioMicrophoneComponent? component = null) { }

    public void ToggleRadioSpeaker(EntityUid uid, EntityUid user, bool quiet = false, RadioSpeakerComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        // no matter how the component is toggled, we want the cooldown to be enforced (funky station)
        if (TryComp<UseDelayComponent>(uid, out var delayComp) && component.Cooldown.HasValue)
        {
            if (_delays.IsDelayed((uid, delayComp)))
                return;

            _delays.SetLength((uid, delayComp), component.Cooldown.Value);
            _delays.TryResetDelay((uid, delayComp));
        }

        _actions.SetToggled(component.ActionEntity, !component.Enabled); // funky
        SetSpeakerEnabled(uid, user, !component.Enabled, quiet, component);
    }

    public void SetSpeakerEnabled(EntityUid uid, EntityUid? user, bool enabled, bool quiet = false, RadioSpeakerComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        // if we're switching the speaker on, make sure we don't need power first (funky)
        if (enabled && component.PowerRequired && !_powerReceiverSystem.IsPowered(uid))
            return;

        // If the mic is on when the speaker is turned off, turn the mic off (Funkystation)
        if (!enabled &&
            TryComp<RadioMicrophoneComponent>(uid, out var mic)
            && mic.SpeakerRequired
            && mic.Enabled)
        {
            _actions.SetToggled(mic.ActionEntity, false);
            SetMicrophoneEnabled(uid, user, false, true, mic);
        }

        component.Enabled = enabled;
        Dirty(uid, component);

        if (!quiet && user != null)
        {
            var state = Loc.GetString(component.Enabled ? "handheld-radio-component-on-state" : "handheld-radio-component-off-state");
            var message = Loc.GetString("handheld-radio-component-on-use", ("radioState", state));
            _popup.PopupClient(message, uid, user.Value); // funky - show the popup over the radio, not the player
        }

        _appearance.SetData(uid, RadioDeviceVisuals.Speaker, component.Enabled);
        if (component.Enabled)
            EnsureComp<ActiveRadioComponent>(uid).Channels.UnionWith(component.Channels);
        else
            RemCompDeferred<ActiveRadioComponent>(uid);
    }
    #endregion
}

