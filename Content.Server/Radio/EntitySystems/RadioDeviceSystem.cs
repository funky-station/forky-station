using System.Linq;
using Content.Server.Chat.Systems;
using Content.Server.Interaction;
using Content.Server.Popups;
using Content.Server.Power.EntitySystems;
using Content.Shared.Chat;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Power;
using Content.Shared.Radio;
using Content.Shared._Funkystation.Radio;
using Content.Shared.Radio.Components;
using Content.Shared.Radio.EntitySystems;
using Content.Shared.Speech;
using Content.Shared.Speech.Components;
using Robust.Server.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.Radio.EntitySystems;

/// <summary>
///     This system handles radio speakers and microphones (which together form a hand-held radio).
/// </summary>
public sealed partial class RadioDeviceSystem : SharedRadioDeviceSystem
{
    [Dependency] private IPrototypeManager _protoMan = default!;
    [Dependency] private PopupSystem _popup = default!;
    [Dependency] private ChatSystem _chat = default!;
    [Dependency] private RadioSystem _radio = default!;
    [Dependency] private InteractionSystem _interaction = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private AudioSystem _audio = null!; // funky - sound effects for radios

    // Used to prevent a shitter from using a bunch of radios to spam chat.
    private HashSet<(string, EntityUid, RadioChannelPrototype)> _recentlySent = new();

    // TODO: improve volume ui
    // TODO: perhaps relay incoming radio messages into your chat like a headset when held (this should probably be implemented as a new component, separately)
    // TODO: ability to change max / min volume / sensitivity on the component (and make UI reflect this)

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RadioMicrophoneComponent, ComponentInit>(OnMicrophoneInit);
        SubscribeLocalEvent<RadioMicrophoneComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<RadioMicrophoneComponent, ActivateInWorldEvent>(OnActivateMicrophone);
        SubscribeLocalEvent<RadioMicrophoneComponent, ListenEvent>(OnListen);
        SubscribeLocalEvent<RadioMicrophoneComponent, ListenAttemptEvent>(OnAttemptListen);
        SubscribeLocalEvent<RadioMicrophoneComponent, PowerChangedEvent>(OnPowerChanged);

        SubscribeLocalEvent<RadioSpeakerComponent, ComponentInit>(OnSpeakerInit);
        SubscribeLocalEvent<RadioSpeakerComponent, ActivateInWorldEvent>(OnActivateSpeaker);
        SubscribeLocalEvent<RadioSpeakerComponent, RadioReceiveEvent>(OnReceiveRadio);
        SubscribeLocalEvent<RadioSpeakerComponent, ExaminedEvent>(OnExamine); // funky - display state when examined

        SubscribeLocalEvent<RadioSpeakerComponent, RadioVolumeSliderMessage>(OnRadioVolumeChanged); // funky - radio volume ui
        SubscribeLocalEvent<RadioMicrophoneComponent, RadioVolumeSliderMessage>(OnRadioSensitivityChanged); // funky - radio volume ui

        SubscribeLocalEvent<IntercomComponent, EncryptionChannelsChangedEvent>(OnIntercomEncryptionChannelsChanged);
        SubscribeLocalEvent<IntercomComponent, ToggleIntercomMicMessage>(OnToggleIntercomMic);
        SubscribeLocalEvent<IntercomComponent, ToggleIntercomSpeakerMessage>(OnToggleIntercomSpeaker);
        SubscribeLocalEvent<IntercomComponent, SelectIntercomChannelMessage>(OnSelectIntercomChannel);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        _recentlySent.Clear();
    }


    #region Component Init
    private void OnMicrophoneInit(EntityUid uid, RadioMicrophoneComponent component, ComponentInit args)
    {
        if (component.Enabled)
            EnsureComp<ActiveListenerComponent>(uid).Range = component.ListenRange;
        else
            RemCompDeferred<ActiveListenerComponent>(uid);
    }

    private void OnSpeakerInit(EntityUid uid, RadioSpeakerComponent component, ComponentInit args)
    {
        if (component.Enabled)
            EnsureComp<ActiveRadioComponent>(uid).Channels.UnionWith(component.Channels);
        else
            RemCompDeferred<ActiveRadioComponent>(uid);
    }
    #endregion

    #region Toggling
    // BEGIN Funkystation
    // toggling the radio as a whole (the speaker) with Z if the speaker is off or there is no mic present (otherwise, toggle the mic)
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
    // toggling the microphone with Z if the speaker is turned on
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
    // END Funkystation
    private void OnPowerChanged(EntityUid uid, RadioMicrophoneComponent component, ref PowerChangedEvent args)
    {
        if (args.Powered)
            return;
        SetMicrophoneEnabled(uid, null, false, true, component);
    }


    public override void SetMicrophoneEnabled(EntityUid uid, EntityUid? user, bool enabled, bool quiet = false, RadioMicrophoneComponent? component = null)
    {
        if (!Resolve(uid, ref component, false))
            return;

        if (component.PowerRequired && !this.IsPowered(uid, EntityManager))
            return;

        component.Enabled = enabled;

        if (!quiet && user != null)
        {
            _audio.PlayPvs(enabled ? component.ToggleOnSound : component.ToggleOffSound, uid); // funky - radio sfx
            var state = Loc.GetString(component.Enabled ? "handheld-radio-component-on-state" : "handheld-radio-component-off-state");
            var message = Loc.GetString("handheld-radio-component-mic-toggle", ("radioState", state)); // funky - popup for mic toggle
            _popup.PopupEntity(message, uid, user.Value); //funky
        }

        _appearance.SetData(uid, RadioDeviceVisuals.Broadcasting, component.Enabled);
        if (component.Enabled)
            EnsureComp<ActiveListenerComponent>(uid).Range = component.ListenRange;
        else
            RemCompDeferred<ActiveListenerComponent>(uid);
    }

    #endregion
    // funky addition - showing the speaker's state when examined separately from the microphone
    private void OnExamine(EntityUid uid, RadioSpeakerComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        var state = Loc.GetString(component.Enabled
            ? "handheld-radio-component-on-state"
            : "handheld-radio-component-off-state");

        var color = component.Enabled
            ? Color.LightBlue
            : Color.Red;

        using (args.PushGroup(nameof(RadioSpeakerComponent), priority: 1))
        {
            args.PushMarkup(Loc.GetString("handheld-radio-component-speaker-examine",
                ("speakerState", state),
                ("color", color)));
        }
    }
    // showing the microphone's state when examined
    private void OnExamine(EntityUid uid, RadioMicrophoneComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        var proto = _protoMan.Index<RadioChannelPrototype>(component.BroadcastChannel);

        var state = Loc.GetString(component.Enabled //funky
            ? "handheld-radio-component-on-state"
            : "handheld-radio-component-off-state");

        var color = component.Enabled //funky
            ? Color.LightBlue
            : Color.Red;

        using (args.PushGroup(nameof(RadioMicrophoneComponent), priority: 0))
        {
            args.PushMarkup(Loc.GetString("handheld-radio-component-mic-examine", //funky
                ("micState", state),
                ("color", color)));
            args.PushMarkup(Loc.GetString("handheld-radio-component-on-examine", ("frequency", proto.Frequency)));
            args.PushMarkup(Loc.GetString("handheld-radio-component-chennel-examine",
                ("channel", proto.LocalizedName)));
        }
    }

    private void OnListen(EntityUid uid, RadioMicrophoneComponent component, ListenEvent args)
    {
        if (HasComp<RadioSpeakerComponent>(args.Source))
            return; // no feedback loops please.

        var channel = _protoMan.Index<RadioChannelPrototype>(component.BroadcastChannel)!;
        if (_recentlySent.Add((args.Message, args.Source, channel)))
            _radio.SendRadioMessage(args.Source, args.Message, channel, uid);
    }

    private void OnAttemptListen(EntityUid uid, RadioMicrophoneComponent component, ListenAttemptEvent args)
    {
        if (component.PowerRequired && !this.IsPowered(uid, EntityManager)
            || component.UnobstructedRequired && !_interaction.InRangeUnobstructed(args.Source, uid, 0))
        {
            args.Cancel();
        }
    }

    // TODO: im glad ghosts dont get their chat clogged, but what about lots of adjacent radios!
    private void OnReceiveRadio(EntityUid uid, RadioSpeakerComponent component, ref RadioReceiveEvent args)
    {
        if (uid == args.RadioSource)
            return;

        var nameEv = new TransformSpeakerNameEvent(args.MessageSource, Name(args.MessageSource));
        RaiseLocalEvent(args.MessageSource, nameEv);

        var name = Loc.GetString("speech-name-relay",
            ("speaker", Name(uid)),
            ("originalName", nameEv.VoiceName));

        // log to chat so people can identity the speaker/source, but avoid clogging ghost chat if there are many radios
        _chat.TrySendInGameICMessage(uid,
            args.Message,
            component.Volume <= 2 ? InGameICChatType.Whisper : InGameICChatType.Speak, // funky change - adjustable volume
            ChatTransmitRange.GhostRangeLimit,
            nameOverride: name,
            checkRadioPrefix: false);
    }

    // Funky - radio volume UI events
    private void OnRadioVolumeChanged(Entity<RadioSpeakerComponent> ent, ref RadioVolumeSliderMessage args)
    {
        ent.Comp.Volume = args.Value;
        Dirty(ent);
    }
    private void OnRadioSensitivityChanged(Entity<RadioMicrophoneComponent> ent, ref RadioVolumeSliderMessage args)
    {
        ent.Comp.ListenRange = args.Value;
        Dirty(ent);
        // update the range on the active listener if its present
        if (TryComp<ActiveListenerComponent>(ent, out var listener))
            listener.Range = ent.Comp.ListenRange;
    }

    private void OnIntercomEncryptionChannelsChanged(Entity<IntercomComponent> ent, ref EncryptionChannelsChangedEvent args)
    {
        ent.Comp.SupportedChannels = args.Component.Channels.Select(p => new ProtoId<RadioChannelPrototype>(p)).ToList();

        var channel = args.Component.DefaultChannel;
        if (ent.Comp.CurrentChannel != null && ent.Comp.SupportedChannels.Contains(ent.Comp.CurrentChannel.Value))
            channel = ent.Comp.CurrentChannel;

        SetIntercomChannel(ent, channel);
    }

    private void OnToggleIntercomMic(Entity<IntercomComponent> ent, ref ToggleIntercomMicMessage args)
    {
        if (ent.Comp.RequiresPower && !this.IsPowered(ent, EntityManager))
            return;

        SetMicrophoneEnabled(ent, args.Actor, args.Enabled, true);
        ent.Comp.MicrophoneEnabled = args.Enabled;
        Dirty(ent);
    }

    private void OnToggleIntercomSpeaker(Entity<IntercomComponent> ent, ref ToggleIntercomSpeakerMessage args)
    {
        if (ent.Comp.RequiresPower && !this.IsPowered(ent, EntityManager))
            return;

        SetSpeakerEnabled(ent, args.Actor, args.Enabled, true);
        ent.Comp.SpeakerEnabled = args.Enabled;
        Dirty(ent);
    }

    private void OnSelectIntercomChannel(Entity<IntercomComponent> ent, ref SelectIntercomChannelMessage args)
    {
        if (ent.Comp.RequiresPower && !this.IsPowered(ent, EntityManager))
            return;

        if (!_protoMan.HasIndex<RadioChannelPrototype>(args.Channel) || !ent.Comp.SupportedChannels.Contains(args.Channel))
            return;

        SetIntercomChannel(ent, args.Channel);
    }

    private void SetIntercomChannel(Entity<IntercomComponent> ent, ProtoId<RadioChannelPrototype>? channel)
    {
        ent.Comp.CurrentChannel = channel;

        if (channel == null)
        {
            SetSpeakerEnabled(ent, null, false);
            SetMicrophoneEnabled(ent, null, false);
            ent.Comp.MicrophoneEnabled = false;
            ent.Comp.SpeakerEnabled = false;
            Dirty(ent);
            return;
        }

        if (TryComp<RadioMicrophoneComponent>(ent, out var mic))
            mic.BroadcastChannel = channel.Value;
        if (TryComp<RadioSpeakerComponent>(ent, out var speaker))
            speaker.Channels = new() { channel.Value };
        Dirty(ent);
    }
}
