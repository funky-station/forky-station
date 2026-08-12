using Content.Server._MACRO.Announcements;
using Content.Server.Chat.Systems;
using Content.Shared._Funkystation.CCVar;
using Content.Shared.Chat;
using Content.Shared.Power;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.Communications;

/// <summary>
/// Handles PA announcers, i.e. the things that actually
/// receive announcements, like speakers.
/// </summary>
public sealed partial class PAAnnouncerSystem : EntitySystem
{
    [Dependency] private ChatSystem _chat = null!;
    [Dependency] private IGameTiming _timing = null!;
    [Dependency] private AudioSystem _audio = null!;
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private AnnouncerManager _announcer = null!;

    private const double MessageDelay = 3;
    private const double LongMessageDelay = 5;
    private const float VolumeModifier = -4f;
    // alert sounds can sound kinda weird when there are multiple playing in vicinity of each other and you're walking around
    // this only really matters for the longer ones, like delta alert
    private const float MaxAudioDistance = SharedChatSystem.VoiceRange;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PAAnnouncerComponent, PAAnnouncementEvent>(OnAnnouncementReceived);
        SubscribeLocalEvent<PAAnnouncerComponent, PowerChangedEvent>(OnPowerChanged);

        Subs.CVar(_cfg, PAAnnouncementCVars.PAAnnouncements, OnAnnouncementsCvarChanged, true);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_cfg.GetCVar(PAAnnouncementCVars.PAAnnouncements))
            return;

        var announcers = EntityQueryEnumerator<PAAnnouncerComponent>();
        while (announcers.MoveNext(out var uid, out var comp))
        {
            if (!comp.Enabled)
                continue;
            if (comp.QueuedMessages.Count < 1 || comp.QueuedMessages.Peek().announceTime > _timing.CurTime)
                continue;

            var (line, author, _) = comp.QueuedMessages.Dequeue();

            _chat.TrySendInGameICMessage(uid, line, InGameICChatType.Speak, ChatTransmitRange.GhostRangeLimit, nameOverride: author, checkRadioPrefix: false);
        }
    }

    private void OnAnnouncementReceived(Entity<PAAnnouncerComponent> ent, ref PAAnnouncementEvent args)
    {
        if (!_timing.IsFirstTimePredicted || !ent.Comp.Enabled)
            return;

        if (args.Source != null)
        {
            var nameEv = new TransformSpeakerNameEvent(args.Source.Value, Name(args.Source.Value));
            RaiseLocalEvent(args.Source.Value, nameEv);
        }

        var name = Loc.GetString("pa-announcement-name", ("author", args.Sender));

        // space out multiple announcements coming simultaneously
        if (ent.Comp.QueuedMessages.Count == 0)
            ent.Comp.NextAnnounceTime = _timing.CurTime;
        else
            ent.Comp.NextAnnounceTime += TimeSpan.FromSeconds(LongMessageDelay);

        // queue PA system preamble (this is attributed to the PA system instead of the sender of the announcement)
        if (args.Preamble)
        {
            ent.Comp.QueuedMessages.Enqueue((args.Messages[0], Loc.GetString("pa-system-name"), ent.Comp.NextAnnounceTime));
            ent.Comp.NextAnnounceTime += TimeSpan.FromSeconds(MessageDelay);
        }

        foreach (var line in args.Preamble ? args.Messages[1..] : args.Messages)
        {
            ent.Comp.QueuedMessages.Enqueue((line, name, ent.Comp.NextAnnounceTime));
            ent.Comp.NextAnnounceTime += TimeSpan.FromSeconds(MessageDelay);
        }

        // note that if multiple announcements come in quick succession, the announcement sound will play without waiting
        // for the next announcement or anything like that.
        if (args.PlaySound && !ent.Comp.Quiet)
        {
            var sound = args.AnnouncementSound ?? ent.Comp.AnnouncementSound;
            if (sound == null)
                _announcer.TryGetAnnouncerSound(SharedChatSystem.DefaultAnnouncementSound, out sound);
            _audio.PlayPvs(sound, ent, AudioParams.Default.WithVolume(VolumeModifier).WithMaxDistance(MaxAudioDistance));
        }
    }

    // if pa announcements get disabled in the middle of an announcement being broadcast, we don't want the unsent
    // messages to remain banked up
    private void OnAnnouncementsCvarChanged(bool value)
    {
        if (!value)
        {
            var announcers = EntityQueryEnumerator<PAAnnouncerComponent>();
            while (announcers.MoveNext(out var comp))
            {
                comp.QueuedMessages.Clear();
            }
        }
    }

    private static void OnPowerChanged(Entity<PAAnnouncerComponent> ent, ref PowerChangedEvent args)
    {
        if (ent.Comp.PowerRequired)
        {
            ent.Comp.Enabled = args.Powered;
            ent.Comp.QueuedMessages.Clear();
        }
    }
}
