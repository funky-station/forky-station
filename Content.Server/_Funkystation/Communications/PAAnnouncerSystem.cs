using System.Linq;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Content.Shared._Funkystation.CCVar;
using Content.Shared.Chat;
using Content.Shared.Database;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

// TODO: disable PA announcements when they lose power
namespace Content.Server._Funkystation.Communications
{
    /// <summary>
    /// Contains methods for dispatching PA announcements.
    /// </summary>
    public sealed partial class PASystem : EntitySystem
    {
        [Dependency] private IConfigurationManager _cfg = null!;
        [Dependency] private StationSystem _stationSystem = null!;
        [Dependency] private IAdminLogManager _adminLogger = null!;

        /// <summary>
        /// Dispatches a PA announcement to all receivers.
        /// </summary>
        /// <param name="message">The message being broadcast.</param>
        /// <param name="sender">The name of the person making the announcement.</param>
        /// <param name="source">The EntityUid of the thing that made the announcement.</param>
        /// <param name="preamble">Whether the PA system should make a preamble "Incoming announcement" statement.</param>
        /// <param name="playSound">Whether the PA system should play an announcement sound.</param>
        /// <param name="global">Whether the announcement should broadcast to all grids or just the station of the sender.</param>
        /// <param name="customPreamble">A custom string of text to display instead of the default preamble.</param>
        /// <param name="announcementSound">A custom sound to play instead of whatever the speaker's default is. MAKE SURE IT IS IN MONO!</param>
        public void DispatchPAAnnouncement(
            string message,
            string? sender = null,
            EntityUid? source = null,
            bool preamble = false,
            bool playSound = true,
            bool global = true,
            LocId? customPreamble = null,
            SoundSpecifier? announcementSound = null)
        {
            EntityUid? station = null;
            if (!global)
            {
                station = _stationSystem.GetOwningStation(source);

                if (station == null)
                {
                    // you can't make a station announcement without a station
                    return;
                }
            }
            var lines = FormatPAAnnouncement(message, preamble, customPreamble);
            var author = sender ?? Loc.GetString("comms-console-announcement-unknown-sender");

            var announceEv = new PAAnnouncementEvent(lines, author, source, preamble, playSound, announcementSound);
            // send the announcement to every PAAnnouncer
            var announcers = EntityQueryEnumerator<PAAnnouncerComponent>();
            while (announcers.MoveNext(out var announcer, out var paComp))
            {
                if (!global && _stationSystem.GetOwningStation(announcer) != station)
                    continue;
                if (paComp.Enabled)
                    RaiseLocalEvent(announcer, ref announceEv);
            }

            if (global)
                _adminLogger.Add(LogType.Chat, LogImpact.Low, $"Global station announcement from {sender}: {message}");
            else
                _adminLogger.Add(LogType.Chat, LogImpact.Low, $"Station Announcement on {station} from {sender}: {message}");
        }

        private string[] FormatPAAnnouncement(string message, bool preamble = false, LocId? customPreamble = null)
        {
            var msg = FormattedMessage.EscapeText(message.Trim());

            // add the PA system preamble to the start of the announcement
            if (preamble)
                msg = Loc.GetString(customPreamble ?? "pa-announcement-title")+'\n'+msg;

            // split the announcement into multiple messages by newline, to be sent one after another
            var lines = msg.Split('\n');

            // if the last message of the announcement is too long, split the announcement further by sentence
            // TODO: if the message doesn't contain periods (like, every sentence ends with an exclamation mark)
            //  this doesn't do anything... in most cases this should be fine
            if (lines[^1].Length > _cfg.GetCVar(PAAnnouncementCVars.PAMaxAnnounceMessageLength))
            {
                var lastMessageSplit = lines[^1].Split(". ");
                // assuming the message is using proper punctuation, put the periods back for all except the last sentence
                for (var i = 0; i < lastMessageSplit.Length - 1; i++)
                {
                    lastMessageSplit[i] = Loc.GetString("pa-announcement-long-message-wrap", ("message", lastMessageSplit[i]));
                }
                lines = lines[..^1].Concat(lastMessageSplit).ToArray(); // messy IMO but whatever
            }

            return lines;
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
        [Dependency] private IConfigurationManager _cfg = null!;
        private const double MessageDelay = 3;
        private const double LongMessageDelay = 5;
        private const float VolumeModifier = -4f;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<PAAnnouncerComponent, PAAnnouncementEvent>(OnAnnouncementReceived);
            Subs.CVar(_cfg, PAAnnouncementCVars.PAAnnouncements, OnAnnouncementsCvarChanged, true);
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

            if (!_cfg.GetCVar(PAAnnouncementCVars.PAAnnouncements))
                return;

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
    }

    /// <summary>
    /// Raised on all PAComponents when an announcement is made with
    /// cvar "funkystation.chat.pa_announcements".
    /// </summary>
    /// <param name="Messages">An array of messages to be sent by PA speakers.</param>
    /// <param name="Sender">The name of the person making the announcement.</param>
    /// <param name="Source">The EntityUid of the thing that made the announcement.</param>
    /// <param name="Preamble">Whether the first message of the announcement should be treated as a preamble "Incoming announcement" statement.</param>
    /// <param name="PlaySound">Whether the PA system should play an announcement sound.</param>
    /// <param name="AnnouncementSound">A custom sound to play instead of whatever the speaker's default is.</param>
    [ByRefEvent]
    public readonly record struct PAAnnouncementEvent(
        string[] Messages,
        string Sender,
        EntityUid? Source,
        bool Preamble = false,
        bool PlaySound = true,
        SoundSpecifier? AnnouncementSound = null);
}
