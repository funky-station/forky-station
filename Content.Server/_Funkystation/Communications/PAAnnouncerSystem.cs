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
            var maxLength = _cfg.GetCVar(CCVars.ChatMaxAnnouncementLength);
            // var msg = SanitizePAAnnouncement(message.Message, maxLength);
            var msg = message.Message.Trim();

            var author = Loc.GetString("comms-console-announcement-unknown-sender");

            var lines = msg.Split('\n');

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

            Log.Debug("raise announce");
            var announce = new PAAnnouncementEvent(lines, author, message.Actor);

            var announcers = EntityQueryEnumerator<PAAnnouncerComponent>();
            while (announcers.MoveNext(out var announcer, out var paComp))
            {
                Log.Debug("send message");
                RaiseLocalEvent(announcer, ref announce);
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

        private readonly Queue<(string line, string author, TimeSpan announceTime)> _queuedMessages = new();

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<PAAnnouncerComponent, PAAnnouncementEvent>(OnAnnouncementReceived);
        }

        private void OnAnnouncementReceived(EntityUid uid, PAAnnouncerComponent comp, ref PAAnnouncementEvent args)
        {
            if (!_timing.IsFirstTimePredicted)
                return;

            if (_queuedMessages.Count > 0)
                return;

            Log.Debug("announcement raised");
            var nameEv = new TransformSpeakerNameEvent(args.Sender, Name(args.Sender));
            RaiseLocalEvent(args.Sender, nameEv);

            var name = Loc.GetString("pa-announcement-name", ("author", args.Author));
            // taking inspiration from https://github.com/space-wizards/space-station-14/pull/42806
            var totalChatCooldown = TimeSpan.Zero;
            foreach (var line in args.Message)
            {
                _queuedMessages.Enqueue((line, name, _timing.CurTime + totalChatCooldown));
                totalChatCooldown += TimeSpan.FromSeconds(3); // TODO: put this in the component
            }
        }

        public override void Update(float frameTime)
        {
            // taking inspiration from https://github.com/space-wizards/space-station-14/pull/42806
            base.Update(frameTime);

            var curTime =  _timing.CurTime;

            if (_queuedMessages.Count < 1 || _queuedMessages.Peek().announceTime > curTime)
                return;

            Log.Debug("mesasage received");

            var (line, author, _) = _queuedMessages.Dequeue();

            var announcers = EntityQueryEnumerator<PAAnnouncerComponent>();
            while (announcers.MoveNext(out var uid, out var comp))
            {
                Log.Debug("send message");
                _chat.TrySendInGameICMessage(uid, line, InGameICChatType.Speak, ChatTransmitRange.GhostRangeLimit, nameOverride: author, checkRadioPrefix: false);
            }

        }
    }

    [ByRefEvent]
    public readonly record struct PAAnnouncementEvent(string[] Message, string Author, EntityUid Sender);
}
