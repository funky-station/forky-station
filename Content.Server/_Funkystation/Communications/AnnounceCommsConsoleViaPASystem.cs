using Content.Server._Funkystation.Communications;
using Content.Shared.Communications;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Database;

namespace Content.Server.Communications
{
    public sealed partial class CommunicationsConsoleSystem
    {
        [Dependency] private PASystem _paSystem = null!;
        private void AnnounceCommsConsoleViaPASystem(EntityUid uid,
            CommunicationsConsoleComponent comp,
            CommunicationsConsoleAnnounceMessage message,
            bool paExclusive)
        {
            Loc.TryGetString(comp.Title, out var title);
            title ??= comp.Title;
            title += " Announcement";

            Loc.TryGetString(comp.Preamble, out var preamble);

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

            if (paExclusive)
            {
                // only set the cooldown if announcements are exclusively handled by the PA system, otherwise we'll accidentally block the normal announcement
                comp.AnnouncementCooldownRemaining = comp.Delay;
                UpdateCommsConsoleInterface(uid, comp);

                // i don't know if CommunicationConsoleAnnouncementEvent is actually used for anything, so we'll do it identically
                // to a normal announcement just to be safe
                var maxLength = _cfg.GetCVar(CCVars.ChatMaxAnnouncementLength);
                var msg = SharedChatSystem.SanitizeAnnouncement(message.Message, maxLength);
                var ev = new CommunicationConsoleAnnouncementEvent(uid, comp, msg, message.Actor);
                RaiseLocalEvent(ref ev);
            }

            _announcer.TryGetAnnouncerSound(comp.Sound, out var sound);

            _paSystem.DispatchPAAnnouncement(
                message: message.Message,
                sender: author,
                source: message.Actor,
                preamble: true,
                playSound: paExclusive, // we don't want to double up on announcement sounds being played globally and through the PA speakers
                global: comp.Global,
                customPreamble: preamble,
                announcementSound: sound,
                colorOverride: comp.Color);

            _adminLogger.Add(LogType.Chat, LogImpact.Low, $"{ToPrettyString(message.Actor):player} has sent the following station announcement: {message.Message}");
        }
    }
}
