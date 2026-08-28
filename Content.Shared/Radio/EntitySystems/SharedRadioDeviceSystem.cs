using Content.Shared.Actions;
using Content.Shared.Interaction;
using System.Linq;
using Content.Shared._Funkystation.Radio;
using Content.Shared.Chat;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Radio.Components;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Robust.Shared.Utility;
using Content.Shared.Speech;
using Content.Shared.Speech.Components;
using Robust.Shared.Audio.Systems;

using Robust.Shared.Prototypes;

namespace Content.Shared.Radio.EntitySystems;

/// <summary>
/// This system handles radio speakers and microphones (which together form a hand-held radio).
/// </summary>
public abstract partial class SharedRadioDeviceSystem : EntitySystem
{
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedChatSystem _chat = default!;
    [Dependency] private SharedRadioSystem _radio = default!;
    [Dependency] private SharedPowerReceiverSystem _power = default!;
    [Dependency] private SharedInteractionSystem _interaction = default!;
    [Dependency] private SharedActionsSystem _actions = null!; // Funky - add speaker/mic toggle actions
    [Dependency] private UseDelaySystem _delays = null!; // Funky - toggle cooldown
    [Dependency] private SharedAudioSystem _audio = null!; // funky - sound effects for radios

    // funky - colors for examine markup
    private Color EnabledColor = Color.FromHex("#31843E");
    private Color DisabledColor = Color.FromHex("#BB3232");

    // Used to prevent a shitter from using a bunch of radios to spam chat.
    private readonly HashSet<(string, EntityUid, RadioChannelPrototype)> _recentlySent = [];

    /// <inheritdoc/>
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        _recentlySent.Clear();
    }

    #region Component Init

    [SubscribeLocalEvent]
    private void OnMicrophoneInit(Entity<RadioMicrophoneComponent> ent, ref ComponentInit args)
    {
        if (ent.Comp.Enabled)
            EnsureComp<ActiveListenerComponent>(ent).Range = ent.Comp.ListenRange;
        else
            RemCompDeferred<ActiveListenerComponent>(ent);
    }

    [SubscribeLocalEvent]
    private void OnSpeakerInit(Entity<RadioSpeakerComponent> ent, ref ComponentInit args)
    {
        if (ent.Comp.Enabled)
            EnsureComp<ActiveRadioComponent>(ent).Channels.UnionWith(ent.Comp.Channels);
        else
            RemCompDeferred<ActiveRadioComponent>(ent);
    }

    #endregion

    #region Toggling

    // funky - toggling the microphone with Z if the speaker is turned on
    [SubscribeLocalEvent]
    private void OnActivateMicrophone(Entity<RadioMicrophoneComponent> ent, ref ActivateInWorldEvent args)
    {
        if (args.Handled) // we dont want the mic to turn on at the same time as the speaker (does this actually ensure that?)
            return;

        if (!args.Complex)
            return;

        if (!ent.Comp.ToggleOnInteract)
            return;
        // fail if this radio requires that the speaker be switched on and there is no speaker / its switched off
        if (ent.Comp.SpeakerRequired && (!TryComp<RadioSpeakerComponent>(ent, out var speaker) || !speaker.Enabled))
            return;

        ToggleRadioMicrophone(ent.AsNullable(), args.User, args.Handled);
        args.Handled = true;
    }

    [SubscribeLocalEvent]
    private void OnActivateSpeaker(Entity<RadioSpeakerComponent> ent, ref ActivateInWorldEvent args)
    {
        if (!args.Complex)
            return;

        if (!ent.Comp.ToggleOnInteract)
            return;
        // funky - if the speaker is on, interacting should toggle the mic instead (if its present)
        if (ent.Comp.Enabled && HasComp<RadioMicrophoneComponent>(ent))
            return;

        ToggleRadioSpeaker(ent.AsNullable(), args.User, args.Handled);
        args.Handled = true;
    }

    [SubscribeLocalEvent]
    private void OnPowerChanged(Entity<RadioMicrophoneComponent> ent, ref PowerChangedEvent args)
    {
        if (args.Powered)
            return;

        SetMicrophoneEnabled(ent.AsNullable(), null, false, true);
    }

    /// <summary>
    /// Enables or disables a radio microphone.
    /// </summary>
    /// <param name="ent">The entity with the microphone.</param>
    /// <param name="user">The entity toggling the microphone, if any.</param>
    /// <param name="enabled">Whether the microphone should be enabled.</param>
    /// <param name="quiet">Whether to suppress the user-facing popup.</param>
    public void SetMicrophoneEnabled(
        Entity<RadioMicrophoneComponent?> ent,
        EntityUid? user,
        bool enabled,
        bool quiet = false
    )
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return;

        if (ent.Comp.PowerRequired && !_power.IsPowered(ent.Owner))
            return;

        ent.Comp.Enabled = enabled;
        Dirty(ent);

        if (!quiet && user != null)
        {
            _audio.PlayPredicted(enabled ? ent.Comp.ToggleOnSound : ent.Comp.ToggleOffSound, ent, user); // funky - radio sfx
            var state = Loc.GetString(ent.Comp.Enabled
                ? "handheld-radio-component-on-state"
                : "handheld-radio-component-off-state");
            var message = Loc.GetString("handheld-radio-component-mic-toggle", ("radioState", state)); // funky
            _popup.PopupEntity(message, user.Value, user.Value);
        }

        _appearance.SetData(ent, RadioDeviceVisuals.Broadcasting, ent.Comp.Enabled);
        if (ent.Comp.Enabled)
            EnsureComp<ActiveListenerComponent>(ent).Range = ent.Comp.ListenRange;
        else
            RemCompDeferred<ActiveListenerComponent>(ent);
    }

    /// <summary>
    /// Toggles a radio microphone.
    /// </summary>
    /// <param name="ent">The entity with the microphone.</param>
    /// <param name="user">The entity toggling the microphone.</param>
    /// <param name="quiet">Whether to suppress the user-facing popup.</param>
    public void ToggleRadioMicrophone(
        Entity<RadioMicrophoneComponent?> ent,
        EntityUid user,
        bool quiet = false
    )
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return;

        // no matter how the component is toggled, we want the cooldown to be enforced (funky station)
        if (TryComp<UseDelayComponent>(ent, out var delayComp) && ent.Comp.Cooldown.HasValue)
        {
            if (_delays.IsDelayed((ent, delayComp)))
                return;

            _delays.SetLength((ent, delayComp), ent.Comp.Cooldown.Value);
            _delays.TryResetDelay((ent, delayComp));

        }

        _actions.SetToggled(ent.Comp.ActionEntity, !ent.Comp.Enabled); // funky
        SetMicrophoneEnabled(ent, user, !ent.Comp.Enabled, quiet);
    }

    /// <summary>
    /// Toggles a radio speaker.
    /// </summary>
    /// <param name="ent">The entity with the speaker.</param>
    /// <param name="user">The entity toggling the speaker.</param>
    /// <param name="quiet">Whether to suppress the user-facing popup.</param>
    public void ToggleRadioSpeaker(
        Entity<RadioSpeakerComponent?> ent,
        EntityUid user,
        bool quiet = false
    )
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return;

        // no matter how the component is toggled, we want the cooldown to be enforced (funky station)
        if (TryComp<UseDelayComponent>(ent, out var delayComp) && ent.Comp.Cooldown.HasValue)
        {
            if (_delays.IsDelayed((ent, delayComp)))
                return;

            _delays.SetLength((ent, delayComp), ent.Comp.Cooldown.Value);
            _delays.TryResetDelay((ent, delayComp));
        }

        _actions.SetToggled(ent.Comp.ActionEntity, !ent.Comp.Enabled); // funky
        SetSpeakerEnabled(ent, user, !ent.Comp.Enabled, quiet);
    }

    /// <summary>
    /// Enables or disables a radio speaker.
    /// </summary>
    /// <param name="ent">The entity with the speaker.</param>
    /// <param name="user">The entity toggling the speaker, if any.</param>
    /// <param name="enabled">Whether the speaker should be enabled.</param>
    /// <param name="quiet">Whether to suppress the user-facing popup.</param>
    public void SetSpeakerEnabled(
        Entity<RadioSpeakerComponent?> ent,
        EntityUid? user,
        bool enabled,
        bool quiet = false
    )
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return;

        // if we're switching the speaker on, make sure we don't need power first (funky)
        if (enabled && ent.Comp.PowerRequired && !_power.IsPowered(ent.Owner))
            return;

        // If the mic is on when the speaker is turned off, turn the mic off (Funkystation)
        if (!enabled &&
            TryComp<RadioMicrophoneComponent>(ent, out var mic)
            && mic.SpeakerRequired
            && mic.Enabled)
        {
            _actions.SetToggled(mic.ActionEntity, false);
            SetMicrophoneEnabled((ent, mic), user, false, true);
        }

        ent.Comp.Enabled = enabled;
        Dirty(ent);

        if (!quiet && user != null)
        {
            var state = Loc.GetString(ent.Comp.Enabled
                ? "handheld-radio-component-on-state"
                : "handheld-radio-component-off-state");
            var message = Loc.GetString("handheld-radio-component-on-use", ("radioState", state));
            _popup.PopupEntity(message, ent, user.Value); // funky - show the popup over the radio, not the player
        }

        _appearance.SetData(ent, RadioDeviceVisuals.Speaker, ent.Comp.Enabled);
        if (ent.Comp.Enabled)
            EnsureComp<ActiveRadioComponent>(ent).Channels.UnionWith(ent.Comp.Channels);
        else
            RemCompDeferred<ActiveRadioComponent>(ent);
    }

    #endregion

    // this entire region is funky additions
    #region Funky - Verbs and actions

    [SubscribeLocalEvent]
    private void AddSpeakerToggleVerb(Entity<RadioSpeakerComponent> ent, ref GetVerbsEvent<AlternativeVerb> args) // Funkystation
    {
        if(!args.CanAccess || !args.CanInteract)
            return;

        if (!ent.Comp.ToggleOnInteract)
            return;

        var user = args.User;

        AlternativeVerb verb = new()
        {
            Act = () =>
            {
                ToggleRadioSpeaker(ent.AsNullable(), user, false);
            },
            Text = Loc.GetString("handheld-radio-component-power-verb"),
            Priority = 1,
            Icon = new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/Spare/poweronoff.svg.192dpi.png")), // TODO: unhardcode
            Message = Loc.GetString("handheld-radio-component-speaker-desc"),
        };
        args.Verbs.Add(verb);
    }

    [SubscribeLocalEvent]
    private void AddMicToggleVerb(Entity<RadioMicrophoneComponent> ent, ref GetVerbsEvent<AlternativeVerb> args) // Funkystation
    {
        if(!args.CanAccess || !args.CanInteract)
            return;

        if (!ent.Comp.ToggleOnInteract)
            return;

        var disabled = false;
        var message = Loc.GetString("handheld-radio-component-mic-desc");
        if (ent.Comp.SpeakerRequired && (!TryComp<RadioSpeakerComponent>(ent, out var speaker) || !speaker.Enabled))
        {
            disabled = true;
            message = Loc.GetString("handheld-radio-component-mic-desc-disabled");
        }

        var user = args.User;

        AlternativeVerb verb = new()
        {
            Act = () =>
            {
                ToggleRadioMicrophone(ent.AsNullable(), user, false);
            },
            Text = Loc.GetString("handheld-radio-component-mic-verb"),
            Priority = 0,
            Icon = new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/signal.svg.192dpi.png")), // TODO: unhardcode this
            Disabled = disabled,
            Message = message,
        };
        args.Verbs.Add(verb);
    }

    [SubscribeLocalEvent]
    private void OnSpeakerToggleAction(Entity<RadioSpeakerComponent> ent, ref ToggleRadioSpeakerEvent args)
    {
        if (args.Handled)
            return;

        if (!ent.Comp.ToggleOnInteract)
            return;

        ToggleRadioSpeaker(ent.AsNullable(), args.Performer, false);
        args.Handled = true;
    }

    [SubscribeLocalEvent]
    private void OnMicrophoneToggleAction(Entity<RadioMicrophoneComponent> ent, ref ToggleRadioMicrophoneEvent args)
    {
        if (args.Handled)
            return;

        if (!ent.Comp.ToggleOnInteract)
            return;

        if (ent.Comp.SpeakerRequired && (!TryComp<RadioSpeakerComponent>(ent, out var speaker) || !speaker.Enabled))
        {
            _popup.PopupEntity(Loc.GetString("handheld-radio-component-mic-desc-disabled"), ent, args.Performer);
            return;
        }

        ToggleRadioMicrophone(ent.AsNullable(), args.Performer, false);
        args.Handled = true;
    }

    [SubscribeLocalEvent]
    private void OnGetActions(Entity<RadioSpeakerComponent> ent, ref GetItemActionsEvent args)
    {
        _actions.AddAction(args.User, ref ent.Comp.ActionEntity, ent.Comp.ActionId, ent);
        if (ent.Comp.Cooldown.HasValue)
            _actions.SetUseDelay(ent.Comp.ActionEntity, ent.Comp.Cooldown.Value); // overrides whatever cooldown the base action has
    }

    [SubscribeLocalEvent]
    private void OnGetActions(Entity<RadioMicrophoneComponent> ent, ref GetItemActionsEvent args)
    {
        _actions.AddAction(args.User, ref ent.Comp.ActionEntity, ent.Comp.ActionId, ent);
        if (ent.Comp.Cooldown.HasValue)
            _actions.SetUseDelay(ent.Comp.ActionEntity, ent.Comp.Cooldown.Value); // overrides whatever cooldown the base action has
    }
    #endregion

    #region Examining

    // funky addition - showing the speaker's state when examined separately from the microphone
    [SubscribeLocalEvent]
    private void OnExamine(Entity<RadioSpeakerComponent> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        var state = Loc.GetString(ent.Comp.Enabled
            ? "handheld-radio-component-on-state"
            : "handheld-radio-component-off-state");

        var color = ent.Comp.Enabled
            ? EnabledColor
            : DisabledColor;

        using (args.PushGroup(nameof(RadioSpeakerComponent), priority: 1))
        {
            args.PushMarkup(Loc.GetString("handheld-radio-component-speaker-examine",
                ("speakerState", state),
                ("color", color)));
            if (HasComp<EncryptionKeyHolderComponent>(ent))
                return;
            // some extra markup we dont want to overlap with encryption key markup
            if (ent.Comp.Channels.Count > 1)
            {
                args.PushMarkup(Loc.GetString("handheld-radio-component-speaker-freq-multiple"));
                foreach (var channel in ent.Comp.Channels)
                {
                    var proto = ProtoMan.Index(channel);
                    // visually mimicking intercoms / encryption key holders
                    args.PushMarkup(Loc.GetString("handheld-radio-component-freq",
                        ("color", proto.Color),
                        ("id", proto.LocalizedName),
                        ("freq", proto.Frequency)));
                }
            }
            // display the singular received channel only if there isnt a mic (reduces clutter)
            else if (!HasComp<RadioMicrophoneComponent>(ent))
            {
                var proto = ProtoMan.Index(ent.Comp.Channels.FirstOrDefault());
                args.PushMarkup(Loc.GetString("handheld-radio-component-speaker-freq",
                    ("color", proto.Color),
                    ("id", proto.LocalizedName),
                    ("freq", proto.Frequency)));
            }
        }
    }

    // showing the microphone's state when examined
    [SubscribeLocalEvent]
    private void OnExamine(Entity<RadioMicrophoneComponent> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        var proto = ProtoMan.Index(ent.Comp.BroadcastChannel);

        // funky edits start
        var state = Loc.GetString(ent.Comp.Enabled
            ? "handheld-radio-component-on-state"
            : "handheld-radio-component-off-state");

        var color = ent.Comp.Enabled
            ? EnabledColor
            : DisabledColor;

        using (args.PushGroup(nameof(RadioMicrophoneComponent), priority: 0))
        {
            args.PushMarkup(Loc.GetString("handheld-radio-component-mic-examine", //funky
                ("micState", state),
                ("color", color)));
            args.PushMarkup(Loc.GetString("handheld-radio-component-mic-freq-examine",
                ("color", proto.Color),
                ("id", proto.LocalizedName),
                ("freq", proto.Frequency)));
        }
        // funky edits end
    }
    #endregion

    #region Chat

    [SubscribeLocalEvent]
    private void OnListen(Entity<RadioMicrophoneComponent> ent, ref ListenEvent args)
    {
        if (HasComp<RadioSpeakerComponent>(args.Source))
            return; // no feedback loops please.

        var channel = ProtoMan.Index(ent.Comp.BroadcastChannel);
        if (_recentlySent.Add((args.Message, args.Source, channel)))
            _radio.SendRadioMessage(args.Source, args.Message, channel, ent.Owner);
    }

    [SubscribeLocalEvent]
    private void OnAttemptListen(Entity<RadioMicrophoneComponent> ent, ref ListenAttemptEvent args)
    {
        if (ent.Comp.PowerRequired && !_power.IsPowered(ent.Owner)
            || ent.Comp.UnobstructedRequired && !_interaction.InRangeUnobstructed(args.Source, ent.Owner, 0))
        {
            args.Cancel();
        }
    }

    // TODO: im glad ghosts dont get their chat clogged, but what about lots of adjacent radios?
    //  if there are many radios near each other, is it possible to prevent all but one from logging to chat?
    //  i gave it a shot, chat system straight up just discards whispers that arent logged. fixing this is out of scope
    [SubscribeLocalEvent]
    private void OnReceiveRadio(Entity<RadioSpeakerComponent> ent, ref RadioReceiveEvent args)
    {
        if (ent.Owner == args.RadioSource)
            return;

        // funky
        if (ent.Comp.PowerRequired && !_power.IsPowered(ent.Owner))
            return;

        var nameEv = new TransformSpeakerNameEvent(args.MessageSource, Name(args.MessageSource));
        RaiseLocalEvent(args.MessageSource, nameEv);

        var name = Loc.GetString("speech-name-relay",
            ("speaker", Name(ent.Owner)),
            ("originalName", nameEv.VoiceName));

        // log to chat so people can identity the speaker/source, but avoid clogging ghost chat if there are many radios
        _chat.TrySendInGameICMessage(ent.Owner,
            args.Message,
            ent.Comp.Volume <= 2 ? InGameICChatType.Whisper : InGameICChatType.Speak, // funky change - adjustable volume
            ChatTransmitRange.GhostRangeLimit,
            nameOverride: name,
            checkRadioPrefix: false);
    }

    #endregion

    #region Funky - radio volume UI events

    [SubscribeLocalEvent]
    // Funky - radio volume UI events
    private void OnRadioVolumeChanged(Entity<RadioSpeakerComponent> ent, ref RadioVolumeSliderMessage args)
    {
        ent.Comp.Volume = Math.Clamp(args.Value, ent.Comp.MinVolume, ent.Comp.MaxVolume);
        Dirty(ent);
    }

    [SubscribeLocalEvent]
    private void OnRadioSensitivityChanged(Entity<RadioMicrophoneComponent> ent, ref RadioVolumeSliderMessage args)
    {
        // if a max range is specified, clamp to it
        if (ent.Comp.MaxRange != null)
            ent.Comp.ListenRange = Math.Clamp(args.Value, ent.Comp.MinRange ?? 1, ent.Comp.MaxRange.Value);
        else
            ent.Comp.ListenRange = args.Value;
        Dirty(ent);
        // update the range on the active listener if its present
        if (TryComp<ActiveListenerComponent>(ent, out var listener))
            listener.Range = ent.Comp.ListenRange;
    }

    #endregion

    #region Intercoms

    [SubscribeLocalEvent]
    private void OnIntercomEncryptionChannelsChanged(
        Entity<IntercomComponent> ent,
        ref EncryptionChannelsChangedEvent args
    )
    {
        ent.Comp.SupportedChannels =
            args.Component.Channels.Select(p => new ProtoId<RadioChannelPrototype>(p)).ToList();

        var channel = args.Component.DefaultChannel;
        if (ent.Comp.CurrentChannel != null && ent.Comp.SupportedChannels.Contains(ent.Comp.CurrentChannel.Value))
            channel = ent.Comp.CurrentChannel;

        SetIntercomChannel(ent, channel);
    }

    [SubscribeLocalEvent]
    private void OnToggleIntercomMic(Entity<IntercomComponent> ent, ref ToggleIntercomMicMessage args)
    {
        if (ent.Comp.RequiresPower && !_power.IsPowered(ent.Owner))
            return;

        SetMicrophoneEnabled(ent.Owner, args.Actor, args.Enabled, true);
        ent.Comp.MicrophoneEnabled = args.Enabled;
        Dirty(ent);
    }

    [SubscribeLocalEvent]
    private void OnToggleIntercomSpeaker(Entity<IntercomComponent> ent, ref ToggleIntercomSpeakerMessage args)
    {
        if (ent.Comp.RequiresPower && !_power.IsPowered(ent.Owner))
            return;

        SetSpeakerEnabled(ent.Owner, args.Actor, args.Enabled, true);
        ent.Comp.SpeakerEnabled = args.Enabled;
        Dirty(ent);
    }

    [SubscribeLocalEvent]
    private void OnSelectIntercomChannel(Entity<IntercomComponent> ent, ref SelectIntercomChannelMessage args)
    {
        if (ent.Comp.RequiresPower && !_power.IsPowered(ent.Owner))
            return;

        if (!ProtoMan.HasIndex<RadioChannelPrototype>(args.Channel) ||
            !ent.Comp.SupportedChannels.Contains(args.Channel))
            return;

        SetIntercomChannel(ent, args.Channel);
    }

    private void SetIntercomChannel(Entity<IntercomComponent> ent, ProtoId<RadioChannelPrototype>? channel)
    {
        ent.Comp.CurrentChannel = channel;

        if (TryComp<RadioMicrophoneComponent>(ent, out var mic))
        {
            if (channel == null)
            {
                SetMicrophoneEnabled(ent.Owner, null, false);
                ent.Comp.MicrophoneEnabled = false;
            }
            else
            {
                mic.BroadcastChannel = channel.Value;
                Dirty(ent, mic);
            }
        }

        if (TryComp<RadioSpeakerComponent>(ent, out var speaker))
        {
            if (channel == null)
            {
                SetSpeakerEnabled(ent.Owner, null, false);
                ent.Comp.SpeakerEnabled = false;
            }
            else
            {
                speaker.Channels = [channel.Value];
                Dirty(ent, speaker);
            }
        }

        Dirty(ent);
    }
    #endregion
}

