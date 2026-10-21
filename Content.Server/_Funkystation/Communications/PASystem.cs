using System.Linq;
using System.Text.RegularExpressions;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Content.Shared._Funkystation.CCVar;
using Content.Shared._Funkystation.Communications;
using Content.Shared.Chat;
using Content.Shared.Database;
using Content.Shared.Station.Components;
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
    [Dependency] private ChatSystem _chat = null!;

    [GeneratedRegex(@"(?<=[\.\!\?])\s")]
    private static partial Regex SpaceAfterSentenceEnd();

    /// <summary>
    /// Dispatches a PA announcement to all receivers.
    /// </summary>
    /// <remarks>
    /// If a targetStation is provided, the announcement will only be dispatched to PA speakers on that station.
    /// Otherwise, all PA speakers will broadcast the announcement.
    /// </remarks>
    /// <param name="announce">The announcement to dispatch.</param>
    /// <param name="preamble">Whether the PA system should make a preamble "Incoming announcement" statement.</param>
    /// <param name="customPreamble">Text to display instead of the default preamble.</param>
    /// <param name="targetStation">A specific station to dispatch the announcement to.</param>
    public void DispatchPAAnnouncement(
        ChatAnnouncement announce,
        bool preamble = false,
        LocId? customPreamble = null,
        EntityUid? targetStation = null)
    {
        var global = !targetStation.HasValue;
        if (targetStation.HasValue && !_stationSystem.GetStationsSet().Contains(targetStation.Value)) // make sure the provided EntityUid is actually a station
        {
            Log.Error($"Tried to dispatch a station announcement to an EntityUid that isn't a station: {ToPrettyString(targetStation)}");
            return;
        }

        var lines = FormatPAAnnouncement(announce.Message, preamble, customPreamble);
        var author = announce.SenderName ?? Loc.GetString("comms-console-announcement-unknown-sender");

        var announceEv = new PAAnnouncementEvent(lines, author, announce.Source, preamble, announce.ShouldPlaySound, announce.AltAnnouncementSound, announce.ColorOverride);

        // send the announcement to every PAAnnouncer
        var announcers = EntityQueryEnumerator<PAAnnouncerComponent>();
        while (announcers.MoveNext(out var announcer, out var paComp))
        {
            if (!global && _stationSystem.GetOwningStation(announcer) != targetStation!.Value)
                continue;

            if (paComp.Enabled)
                RaiseLocalEvent(announcer, ref announceEv);
        }

        if (global)
            _adminLogger.Add(LogType.Chat, LogImpact.Low, $"Global station announcement from {announce.SenderName}: {announce.Message}");
        else
            _adminLogger.Add(LogType.Chat, LogImpact.Low, $"Station Announcement on {ToPrettyString(targetStation)} from {announce.SenderName}: {announce.Message}");
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

        for (var i = 0; i < lines.Length; i++)
        {
            lines[i] = _chat.SanitizeMessageCapital(_chat.SanitizeMessageCapitalizeTheWordI(lines[i]));
        }

        return lines;
    }
}

/// <summary>
/// Raised on all PA announcers when an announcement is made with
/// cvar <see cref="PAAnnouncementCVars.PAEnabled"/> or by using
/// <see cref="PASystem.DispatchPAAnnouncement"/> directly.
/// </summary>
/// <param name="Messages">An array of messages to be sent by PA speakers.</param>
/// <param name="Sender">The name of the person making the announcement.</param>
/// <param name="Source">The EntityUid of the thing that made the announcement.</param>
/// <param name="Preamble">Whether the first message of the announcement should be treated as a preamble "Incoming announcement" statement.</param>
/// <param name="PlaySound">Whether the PA system should play an announcement sound.</param>
/// <param name="AnnouncementSound">A custom sound to play instead of whatever the speaker's default is.</param>
/// <param name="ColorOverride">The color of the announcement's text in the chat window.</param>
[ByRefEvent]
public readonly record struct PAAnnouncementEvent(
    string[] Messages,
    string Sender,
    EntityUid? Source,
    bool Preamble = false,
    bool PlaySound = true,
    SoundSpecifier? AnnouncementSound = null,
    Color? ColorOverride = null);
