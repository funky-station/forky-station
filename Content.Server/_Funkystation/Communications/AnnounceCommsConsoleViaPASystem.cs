using Content.Server._Funkystation.Communications;
using Content.Shared.Communications;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Database;

namespace Content.Server.Communications;

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

        var announcement = new ChatAnnouncement(
            Message: message.Message,
            Source: message.Actor,
            SenderName: author,
            ShouldPlaySound: paExclusive, // if we're not PA exclusive, then the sound will have already been played, and doubling up is undesirable
            AltAnnouncementSound: sound,
            ColorOverride: comp.Color);

        _paSystem.DispatchPAAnnouncement(
            announcement,
            global: comp.Global,
            preamble: true,
            customPreamble: preamble);

        _adminLogger.Add(LogType.Chat, LogImpact.Low, $"{ToPrettyString(message.Actor):player} has sent the following station announcement: {message.Message}");
    }
}

