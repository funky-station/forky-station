using Content.Server._MACRO.Announcements;
using Content.Server._Funkystation.Communications;
using Content.Shared._Funkystation.CCVar;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.Station.Components;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.Chat.Systems;

// todo: mark funky changes on all affected files
// todo: method and ChatAnnouncement documentation
// todo: test it actually works lol

public sealed partial class ChatSystem
{
    [Dependency] private AnnouncerManager _announcer = default!; // macrocosm

    [Dependency] private PASystem _paSystem = null!; // funky - announcements via PA speakers
    /// <inheritdoc />
    public override void DispatchGlobalAnnouncement(ChatAnnouncement chatAnnouncement)
    {
        chatAnnouncement.SenderName ??= Loc.GetString("chat-manager-sender-announcement");

        // funky - redirect announcement to PA speakers
        if (DoPADispatch(chatAnnouncement, true))
            return;

        var wrappedMessage = Loc.GetString("chat-manager-sender-announcement-wrap-message", ("sender", chatAnnouncement.SenderName), ("message", FormattedMessage.EscapeText(chatAnnouncement.Message)));
        _chatManager.ChatMessageToAll(ChatChannel.Radio, chatAnnouncement.Message, wrappedMessage, default, false, true, chatAnnouncement.ColorOverride);

        if (chatAnnouncement.ShouldPlaySound)
        {
            PlayAnnouncementSound(chatAnnouncement);
        }

        _adminLogger.Add(LogType.Chat, LogImpact.Low, $"Global station announcement from {chatAnnouncement.SenderName}: {chatAnnouncement.Message}");
    }

    /// <inheritdoc />
    public override void DispatchFilteredAnnouncement(ChatAnnouncement chatAnnouncement, Filter filter) //funky
    {
        chatAnnouncement.SenderName ??= Loc.GetString("chat-manager-sender-announcement");

        // funky - redirect announcement to PA speakers
        if (DoPADispatch(chatAnnouncement, true))
            return;

        var wrappedMessage = Loc.GetString("chat-manager-sender-announcement-wrap-message", ("sender", chatAnnouncement.SenderName), ("message", FormattedMessage.EscapeText(chatAnnouncement.Message)));
        _chatManager.ChatMessageToManyFiltered(filter, ChatChannel.Radio, chatAnnouncement.Message, wrappedMessage, chatAnnouncement.Source ?? default, false, true, chatAnnouncement.ColorOverride);

        if (chatAnnouncement.ShouldPlaySound)
        {
            PlayAnnouncementSound(chatAnnouncement);
        }

        _adminLogger.Add(LogType.Chat, LogImpact.Low, $"Station Announcement from {chatAnnouncement.SenderName}: {chatAnnouncement.Message}");
    }

    /// <inheritdoc />
    public override void DispatchStationAnnouncement(ChatAnnouncement chatAnnouncement, EntityUid? stationUid = null) // funky
    {
        stationUid ??= _stationSystem.GetOwningStation(chatAnnouncement.Source);

        if (stationUid == null || !TryComp<StationDataComponent>(stationUid, out var stationDataComp))
        {
            // you can't make a station announcement without a station
            return;
        }

        chatAnnouncement.SenderName ??= Loc.GetString("chat-manager-sender-announcement");

        // funky - redirect announcement to PA speakers
        if (DoPADispatch(chatAnnouncement, false, (stationUid.Value, stationDataComp)))
            return;

        var wrappedMessage = Loc.GetString("chat-manager-sender-announcement-wrap-message", ("sender", chatAnnouncement.SenderName), ("message", FormattedMessage.EscapeText(chatAnnouncement.Message)));

        var filter = _stationSystem.GetInStation(stationDataComp);

        _chatManager.ChatMessageToManyFiltered(filter, ChatChannel.Radio, chatAnnouncement.Message, wrappedMessage, chatAnnouncement.Source ?? default, false, true, chatAnnouncement.ColorOverride);

        if (chatAnnouncement.ShouldPlaySound)
        {
            PlayAnnouncementSound(chatAnnouncement);
        }

        _adminLogger.Add(LogType.Chat, LogImpact.Low, $"Station Announcement on {ToPrettyString(stationUid)} from {chatAnnouncement.SenderName}: {chatAnnouncement.Message}");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="chatAnnouncement"></param>
    /// <param name="global"></param>
    /// <param name="targetStation"></param>
    /// <returns>true if pa exclusive</returns> // todo: properly word this
    private bool DoPADispatch(ChatAnnouncement chatAnnouncement, bool global, Entity<StationDataComponent>? targetStation = null)
    {
        if (!chatAnnouncement.BypassPASystem && _configurationManager.GetCVar(PAAnnouncementCVars.PAEnabled))
        {
            var paExclusive = _configurationManager.GetCVar(PAAnnouncementCVars.PAExclusiveAnnouncements);

            _paSystem.DispatchPAAnnouncement(
                chatAnnouncement with { ShouldPlaySound = chatAnnouncement.ShouldPlaySound && paExclusive }, // we don't want to double up on announcement sounds being played globally and through the PA speakers
                preamble: false,
                global: global,
                targetStation: targetStation);

            if (paExclusive)
                return true;
        }
        return false;
    }

    // todo: should filtered / station announcements only play the announcement sound for recipients?
    private void PlayAnnouncementSound(ChatAnnouncement chatAnnouncement)
    {
        var announcementSound = chatAnnouncement.AltAnnouncementSound;
        // Macrocosm edit start - announcer variation
        if (announcementSound == null)
        {
            _announcer.TryGetAnnouncerSound(DefaultAnnouncementSound, out announcementSound);
        }
        _audio.PlayGlobal(announcementSound, Filter.Broadcast(), true, AudioParams.Default.WithVolume(-2f));
        // Macrocosm edit end
    }
}
