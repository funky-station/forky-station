using Content.Shared.Actions;
using Content.Shared.Popups;
using Content.Shared.Radio.Components;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared.Radio.EntitySystems;

public abstract partial class SharedRadioDeviceSystem : EntitySystem
{
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private SharedActionsSystem _actions = null!; // Funky


    public override void Initialize() // funky additions!!
    {
        base.Initialize();

        // BEGIN Funkystation
        SubscribeLocalEvent<RadioSpeakerComponent, GetVerbsEvent<AlternativeVerb>>(AddSpeakerToggleVerb);
        SubscribeLocalEvent<RadioSpeakerComponent, ToggleRadioSpeakerEvent>(OnSpeakerToggleAction);
        SubscribeLocalEvent<RadioSpeakerComponent, GetItemActionsEvent>(OnGetActions);

        SubscribeLocalEvent<RadioMicrophoneComponent, GetVerbsEvent<AlternativeVerb>>(AddMicToggleVerb);
        SubscribeLocalEvent<RadioMicrophoneComponent, ToggleRadioMicrophoneEvent>(OnMicrophoneToggleAction);
        SubscribeLocalEvent<RadioMicrophoneComponent, GetItemActionsEvent>(OnGetActions);
        // END Funkystation
    }

    // this entire region is funky additions
    #region verbs and actions
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

        if (!component.ToggleOnInteract || (component.SpeakerRequired && !Comp<RadioSpeakerComponent>(uid).Enabled))
            return;

        ToggleRadioMicrophone(uid, args.Performer, false, component);
        args.Handled = true;
    }

    private void OnGetActions(EntityUid uid, RadioSpeakerComponent component, GetItemActionsEvent args)
    {
        _actions.AddAction(args.User, ref component.ActionEntity, component.ActionId, uid);
    }
    private void OnGetActions(EntityUid uid, RadioMicrophoneComponent component, GetItemActionsEvent args)
    {
        _actions.AddAction(args.User, ref component.ActionEntity, component.ActionId, uid);
    }
    #endregion
    #region Toggling
    public void ToggleRadioMicrophone(EntityUid uid, EntityUid user, bool quiet = false, RadioMicrophoneComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        _actions.SetToggled(component.ActionEntity, !component.Enabled); // funky
        SetMicrophoneEnabled(uid, user, !component.Enabled, quiet, component);
    }

    // seems to be this way because it can't be predicted on client
    public virtual void SetMicrophoneEnabled(EntityUid uid, EntityUid? user, bool enabled, bool quiet = false, RadioMicrophoneComponent? component = null) { }

    public void ToggleRadioSpeaker(EntityUid uid, EntityUid user, bool quiet = false, RadioSpeakerComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        _actions.SetToggled(component.ActionEntity, !component.Enabled); // funky
        SetSpeakerEnabled(uid, user, !component.Enabled, quiet, component);
    }

    public void SetSpeakerEnabled(EntityUid uid, EntityUid? user, bool enabled, bool quiet = false, RadioSpeakerComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        // If the mic is on when the speaker is turned off, turn the mic off (Funkystation)
        if (!enabled && TryComp<RadioMicrophoneComponent>(uid, out var mic) && mic.Enabled)
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
            _popup.PopupEntity(message, user.Value, user.Value);
        }

        _appearance.SetData(uid, RadioDeviceVisuals.Speaker, component.Enabled);
        if (component.Enabled)
            EnsureComp<ActiveRadioComponent>(uid).Channels.UnionWith(component.Channels);
        else
            RemCompDeferred<ActiveRadioComponent>(uid);
    }
    #endregion
}

