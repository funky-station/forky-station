using Content.Server._Funkystation.Communications;
using Content.Server.Chat.Systems;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Communications;
using Robust.Shared.Timing;

namespace Content.Server.Communications
{
    public sealed partial class CommunicationsConsoleSystem
    {
        private void AnnounceMessageViaPASystem(EntityUid uid,
            CommunicationsConsoleComponent comp,
            CommunicationsConsoleAnnounceMessage message)
        {
            var msg = message.Message.Trim();
            var author = Loc.GetString("comms-console-announcement-unknown-sender");
            var lines = msg.Split('\n');

            // // allow admemes with vv
            // Loc.TryGetString(comp.Title, out var title);
            // title ??= comp.Title;

            if (message.Actor is { Valid: true } mob)
            {
                if (!CanAnnounce(comp))
                    return;

                if (!CanUse(mob, uid))
                {
                    _popupSystem.PopupEntity(Loc.GetString("comms-console-permission-denied"), uid, message.Actor);
                    return;
                }

                author = _identity.GetIdentityShortInfo(mob, uid) ?? author;
            }

            comp.AnnouncementCooldownRemaining = comp.Delay;
            UpdateCommsConsoleInterface(uid, comp);

            var ev = new CommunicationConsoleAnnouncementEvent(uid, comp, msg, message.Actor);
            RaiseLocalEvent(ref ev);

            var announceEv = new PAAnnouncementEvent(lines, author, message.Actor);

            var announcers = EntityQueryEnumerator<PAAnnouncerComponent>();
            while (announcers.MoveNext(out var announcer, out var paComp))
            {
                RaiseLocalEvent(announcer, ref announceEv);
            }

        }

        private string SanitizePAAnnouncement(string message, int maxLength = 0)
        {
            var trimmed = message.Trim();
            if (maxLength > 0 && trimmed.Length > maxLength)
            {
                trimmed = $"{message[..maxLength]}...";
            }

            return trimmed;
        }
    }
}

namespace Content.Server._Funkystation.Communications
{
    public sealed partial class PAAnnouncerSystem : EntitySystem
    {
        [Dependency] private ChatSystem _chat = null!;
        [Dependency] private IGameTiming _timing = null!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<PAAnnouncerComponent, PAAnnouncementEvent>(OnAnnouncementReceived);
        }

        private void OnAnnouncementReceived(EntityUid uid, PAAnnouncerComponent comp, ref PAAnnouncementEvent args)
        {
            if (!_timing.IsFirstTimePredicted)
                return;

            var nameEv = new TransformSpeakerNameEvent(args.Sender, Name(args.Sender));
            RaiseLocalEvent(args.Sender, nameEv);

            var name = Loc.GetString("pa-announcement-name", ("author", args.Author));

            if (comp.QueuedMessages.Count == 0)
                comp.NextAnnounceTime = _timing.CurTime;
            else
                comp.NextAnnounceTime += TimeSpan.FromSeconds(5);

            foreach (var line in args.Message)
            {
                comp.QueuedMessages.Enqueue((line, name, comp.NextAnnounceTime));
                comp.NextAnnounceTime += TimeSpan.FromSeconds(3); // TODO: put this in the component
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

    [ByRefEvent]
    public readonly record struct PAAnnouncementEvent(string[] Message, string Author, EntityUid Sender);
}
