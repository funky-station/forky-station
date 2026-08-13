using System.Linq;
using System.Text.RegularExpressions;
using Content.Server.Administration.Logs;
using Content.Server.Station.Systems;
using Content.Shared._Funkystation.CCVar;
using Content.Shared.Database;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Utility;

namespace Content.Server._Funkystation.Communications;

/// <summary>
/// Handles dispatching PA announcements.
/// </summary>
public sealed partial class PASystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private StationSystem _stationSystem = null!;
    [Dependency] private IAdminLogManager _adminLogger = null!;

    [GeneratedRegex(@"(?<=[\.\!\?])\s")]
    private static partial Regex SpaceAfterSentenceEnd();

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
    /// <param name="colorOverride"></param>
    public void DispatchPAAnnouncement(
        string message,
        string? sender = null,
        EntityUid? source = null,
        bool preamble = false,
        bool playSound = true,
        bool global = true,
        LocId? customPreamble = null,
        SoundSpecifier? announcementSound = null,
        Color? colorOverride = null)
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

        var announceEv = new PAAnnouncementEvent(lines, author, source, preamble, playSound, announcementSound, colorOverride);
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
            msg = Loc.GetString(customPreamble ?? "pa-announcement-preamble")+'\n'+msg;

        // split the announcement into multiple messages by newline, to be sent one after another
        var lines = msg.Split('\n');

        // if the last message of the announcement is too long, split the announcement further by sentence
        if (lines[^1].Length > _cfg.GetCVar(PAAnnouncementCVars.PAMaxAnnounceMessageLength))
        {
            var lastMessageSplit = SpaceAfterSentenceEnd().Split(lines[^1]);
            lines = lines[..^1].Concat(lastMessageSplit).ToArray(); // messy IMO but whatever
        }

        return lines;
    }
}

/// <summary>
/// Raised on all PA announcers when an announcement is made with
/// cvar <see cref="PAAnnouncementCVars.PAAnnouncements"/> or by using
/// <see cref="PASystem.DispatchPAAnnouncement"/> directly.
/// </summary>
/// <param name="Messages">An array of messages to be sent by PA speakers.</param>
/// <param name="Sender">The name of the person making the announcement.</param>
/// <param name="Source">The EntityUid of the thing that made the announcement.</param>
/// <param name="Preamble">Whether the first message of the announcement should be treated as a preamble "Incoming announcement" statement.</param>
/// <param name="PlaySound">Whether the PA system should play an announcement sound.</param>
/// <param name="AnnouncementSound">A custom sound to play instead of whatever the speaker's default is.</param>
/// <param name="ColorOverride"></param>
[ByRefEvent]
public readonly record struct PAAnnouncementEvent(
    string[] Messages,
    string Sender,
    EntityUid? Source,
    bool Preamble = false,
    bool PlaySound = true,
    SoundSpecifier? AnnouncementSound = null,
    Color? ColorOverride = null);
