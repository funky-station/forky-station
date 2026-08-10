using Content.Server._Funkystation.Communications;
using Content.Shared._Funkystation.CCVar;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.Station.Components;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.Chat.Systems;

public sealed partial class ChatSystem
{
    [Dependency] private PASystem _paSystem = null!; // funky - announcements via PA speakers
    /// <inheritdoc />
    /// <funky>If you're developing content for
    /// Funky Station or one of its downstreams
    /// and passing an announcementSound, please
    /// make sure it is in mono when cvar
    /// funkystation.chat.pa_announcements = true.</funky>
    public override void DispatchGlobalAnnouncement(
        string message,
        string? sender = null,
        bool playSound = true,
        SoundSpecifier? announcementSound = null,
        Color? colorOverride = null
        )
    {
        sender ??= Loc.GetString("chat-manager-sender-announcement");

        // funky - redirect announcement to PA speakers
        if (_configurationManager.GetCVar(PAAnnouncementCVars.PAAnnouncements))
        {
            _paSystem.DispatchPAAnnouncement(message, sender, null, false, playSound, true, null, announcementSound);
            return;
        }

        var wrappedMessage = Loc.GetString("chat-manager-sender-announcement-wrap-message", ("sender", sender), ("message", FormattedMessage.EscapeText(message)));
        _chatManager.ChatMessageToAll(ChatChannel.Radio, message, wrappedMessage, default, false, true, colorOverride);
        if (playSound)
        {
            _audio.PlayGlobal(announcementSound ?? DefaultAnnouncementSound, Filter.Broadcast(), true, AudioParams.Default.WithVolume(-2f));
        }
        _adminLogger.Add(LogType.Chat, LogImpact.Low, $"Global station announcement from {sender}: {message}");
    }

    /// <inheritdoc />
    /// <note>Will function identically to
    /// DispatchGlobalAnnouncement when cvar
    /// funkystation.chat.pa_announcements = true.</note>
    /// <funky>If you're developing content for
    /// Funky Station or one of its downstreams
    /// and passing an announcementSound, please
    /// make sure it is in mono when cvar
    /// funkystation.chat.pa_announcements = true.</funky>
    public override void DispatchFilteredAnnouncement(
        Filter filter,
        string message,
        EntityUid? source = null,
        string? sender = null,
        bool playSound = true,
        SoundSpecifier? announcementSound = null,
        Color? colorOverride = null)
    {
        sender ??= Loc.GetString("chat-manager-sender-announcement");

        // funky - redirect announcement to PA speakers
        if (_configurationManager.GetCVar(PAAnnouncementCVars.PAAnnouncements))
        {
            _paSystem.DispatchPAAnnouncement(message, sender, source, false, playSound,  true, null, announcementSound);
            return;
        }

        var wrappedMessage = Loc.GetString("chat-manager-sender-announcement-wrap-message", ("sender", sender), ("message", FormattedMessage.EscapeText(message)));
        _chatManager.ChatMessageToManyFiltered(filter, ChatChannel.Radio, message, wrappedMessage, source ?? default, false, true, colorOverride);
        if (playSound)
        {
            _audio.PlayGlobal(announcementSound ?? DefaultAnnouncementSound, filter, true, AudioParams.Default.WithVolume(-2f));
        }
        _adminLogger.Add(LogType.Chat, LogImpact.Low, $"Station Announcement from {sender}: {message}");
    }

    /// <inheritdoc />
    /// <funky>If you're developing content for
    /// Funky Station or one of its downstreams
    /// and passing an announcementSound, please
    /// make sure it is in mono when cvar
    /// funkystation.chat.pa_announcements = true.</funky>
    public override void DispatchStationAnnouncement(
        EntityUid source,
        string message,
        string? sender = null,
        bool playDefaultSound = true,
        SoundSpecifier? announcementSound = null,
        Color? colorOverride = null)
    {
        sender ??= Loc.GetString("chat-manager-sender-announcement");

        // funky - redirect announcement to PA speakers
        // TODO: station specific announcements
        if (_configurationManager.GetCVar(PAAnnouncementCVars.PAAnnouncements))
        {
            _paSystem.DispatchPAAnnouncement(message, sender, source, false, playDefaultSound, false, null, announcementSound);
            return;
        }

        var wrappedMessage = Loc.GetString("chat-manager-sender-announcement-wrap-message", ("sender", sender), ("message", FormattedMessage.EscapeText(message)));
        var station = _stationSystem.GetOwningStation(source);

        if (station == null)
        {
            // you can't make a station announcement without a station
            return;
        }

        if (!TryComp<StationDataComponent>(station, out var stationDataComp)) return;

        var filter = _stationSystem.GetInStation(stationDataComp);

        _chatManager.ChatMessageToManyFiltered(filter, ChatChannel.Radio, message, wrappedMessage, source, false, true, colorOverride);

        if (playDefaultSound)
        {
            _audio.PlayGlobal(announcementSound ?? DefaultAnnouncementSound, filter, true, AudioParams.Default.WithVolume(-2f));
        }

        _adminLogger.Add(LogType.Chat, LogImpact.Low, $"Station Announcement on {station} from {sender}: {message}");
    }
}
