using Content.Server._Funkystation.Communications;
using Content.Server.Chat.Systems;
using Content.Shared.Communications;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Timing;

// TODO: admin logging
// TODO: station specific announcements
// TODO: the monoification (done, i guess? :( )
namespace Content.Server._Funkystation.Communications
{
    /// <summary>
    /// Contains methods for dispatching PA announcements.
    /// </summary>
    public sealed partial class PASystem : EntitySystem
    {
        /// <summary>
        /// Dispatches a PA announcement to all receivers.
        /// </summary>
        /// <param name="message">The message being broadcast.</param>
        /// <param name="sender">The name of the person making the announcement.</param>
        /// <param name="source">The EntityUid of the thing that made the announcement.</param>
        /// <param name="preamble">Whether the PA system should make a preamble "Incoming announcement" statement.</param>
        /// <param name="playSound">Whether the PA system should play an announcement sound.</param>
        /// <param name="customPreamble">A custom string of text to display instead of the default preamble.</param>
        /// <param name="announcementSound">A custom sound to play instead of whatever the speaker's default is.</param>
        public void DispatchPAAnnouncement(
            string message,
            string? sender = null,
            EntityUid? source = null,
            bool preamble = false,
            bool playSound = true,
            LocId? customPreamble = null,
            SoundSpecifier? announcementSound = null)
        {
            var msg = message.Trim();

            // add the PA system preamble to the start of the announcement
            if (preamble)
                msg = Loc.GetString(customPreamble ?? "pa-announcement-title")+'\n'+msg;

            var author = sender ?? Loc.GetString("comms-console-announcement-unknown-sender");

            // split the announcement into multiple messages by newline, to be sent one after another
            var lines = msg.Split('\n');

            var announceEv = new PAAnnouncementEvent(lines, author, source, preamble, playSound, customPreamble, announcementSound);

            // send the announcement to every PAAnnouncer
            var announcers = EntityQueryEnumerator<PAAnnouncerComponent>();
            while (announcers.MoveNext(out var announcer, out var paComp))
            {
                if (paComp.Enabled)
                    RaiseLocalEvent(announcer, ref announceEv);
            }
        }
    }

    /// <summary>
    /// Handles PA announcers, i.e. the things that actually
    /// receive announcements, like speakers.
    /// </summary>
    public sealed partial class PAAnnouncerSystem : EntitySystem
    {
        [Dependency] private ChatSystem _chat = null!;
        [Dependency] private IGameTiming _timing = null!;
        [Dependency] private AudioSystem _audio = null!;
        private const double MessageDelay = 3;
        private const double LongMessageDelay = 5;
        private const float VolumeModifier = -4f;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<PAAnnouncerComponent, PAAnnouncementEvent>(OnAnnouncementReceived);
        }

        private void OnAnnouncementReceived(Entity<PAAnnouncerComponent> ent, ref PAAnnouncementEvent args)
        {
            if (!_timing.IsFirstTimePredicted)
                return;

            // i'm not even sure if we have to do this, but whatever
            // TODO: figure out if we have to do this
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

            // queue PA system preamble
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
                _audio.PlayPvs(args.AnnouncementSound ?? ent.Comp.AnnouncementSound ?? SharedChatSystem.DefaultAnnouncementSound,
                    ent, AudioParams.Default.WithVolume(VolumeModifier));
            }
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            var announcers = EntityQueryEnumerator<PAAnnouncerComponent>();
            // TODO: iterating through them all like this every update makes me very sad.
            while (announcers.MoveNext(out var uid, out var comp))
            {
                if (comp.QueuedMessages.Count < 1 || comp.QueuedMessages.Peek().announceTime > _timing.CurTime)
                    continue;

                var (line, author, _) = comp.QueuedMessages.Dequeue();

                _chat.TrySendInGameICMessage(uid, line, InGameICChatType.Speak, ChatTransmitRange.GhostRangeLimit, nameOverride: author, checkRadioPrefix: false);
            }
        }
    }

    /// <summary>
    /// Raised on all PAComponents when an announcement is made with
    /// cvar "funkystation.chat.pa_announcements".
    /// </summary>
    /// <param name="Messages">An array of messages to be sent by PA speakers.</param>
    /// <param name="Sender">The name of the person making the announcement.</param>
    /// <param name="Source">The EntityUid of the thing that made the announcement.</param>
    /// <param name="Preamble">Whether the PA system should make a preamble "Incoming announcement" statement.</param>
    /// <param name="PlaySound">Whether the PA system should play an announcement sound.</param>
    /// <param name="CustomPreamble">A custom string of text to display instead of the default preamble.</param>
    /// <param name="AnnouncementSound">A custom sound to play instead of whatever the speaker's default is.</param>
    [ByRefEvent]
    public readonly record struct PAAnnouncementEvent(
        string[] Messages,
        string Sender,
        EntityUid? Source,
        bool Preamble = false,
        bool PlaySound = true,
        LocId? CustomPreamble = null,
        SoundSpecifier? AnnouncementSound = null);
}

namespace Content.Server.Communications
{
    public sealed partial class CommunicationsConsoleSystem
    {
        [Dependency] private PASystem _paSystem = null!;
        private void AnnounceCommsConsoleViaPASystem(EntityUid uid,
            CommunicationsConsoleComponent comp,
            CommunicationsConsoleAnnounceMessage message)
        {
            Loc.TryGetString(comp.Title, out var title);
            title ??= comp.Title;
            title += " Announcement";

            // if we're not going to include the name and ID of the sender, fall back to the "title" (i.e, Communications Console, Syndicate Nuclear Operative)
            var author = comp.AnnounceSentBy ? Loc.GetString("comms-console-announcement-unknown-sender") : title;

            if (message.Actor is { Valid: true } mob)
            {
                if (!CanAnnounce(comp))
                    return;

                if (!CanUse(mob, uid))
                {
                    _popupSystem.PopupEntity(Loc.GetString("comms-console-permission-denied"), uid, message.Actor);
                    return;
                }

                if (comp.AnnounceSentBy)
                    author = _identity.GetIdentityShortInfo(mob, uid) ?? author;
            }

            comp.AnnouncementCooldownRemaining = comp.Delay;
            UpdateCommsConsoleInterface(uid, comp);

            // i don't know if CommunicationConsoleAnnouncementEvent is actually used for anything, so we'll do it identically
            // to a normal announcement just to be safe
            var maxLength = _cfg.GetCVar(CCVars.ChatMaxAnnouncementLength);
            var msg = SharedChatSystem.SanitizeAnnouncement(message.Message, maxLength);
            var ev = new CommunicationConsoleAnnouncementEvent(uid, comp, msg, message.Actor);
            RaiseLocalEvent(ref ev);

            _paSystem.DispatchPAAnnouncement(message.Message, author, message.Actor, true, true, null, comp.Sound);
        }
    }
}
