using Content.Server._MACRO.Announcements;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.Station.Systems;
using Content.Server.StationEvents.Components;
using Content.Shared._Funkystation.CCVar;
using Content.Shared.Database;
using Content.Shared.GameTicking.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Server.StationEvents.Events;

/// <summary>
///     An abstract entity system inherited by all station events for their behavior.
/// </summary>
public abstract partial class StationEventSystem<T> : GameRuleSystem<T> where T : IComponent
{
    [Dependency] protected IAdminLogManager AdminLogManager = default!;
    [Dependency] protected ChatSystem ChatSystem = default!;
    [Dependency] protected SharedAudioSystem Audio = default!;
    [Dependency] protected StationSystem StationSystem = default!;
    [Dependency] private IConfigurationManager _cfg = null!; // funky - pa announcement cvar

    [Dependency] protected AnnouncerManager Announcer = default!; // Macrocosm

    protected ISawmill Sawmill = default!;

    public override void Initialize()
    {
        base.Initialize();

        Sawmill = LogManager.GetSawmill("stationevents");
    }

    /// <inheritdoc/>
    protected override void Added(EntityUid uid, T component, GameRuleComponent gameRule, GameRuleAddedEvent args)
    {
        base.Added(uid, component, gameRule, args);

        if (!TryComp<StationEventComponent>(uid, out var stationEvent))
            return;

        var paAnnouncements = _cfg.GetCVar(PAAnnouncementCVars.PAAnnouncements); // funky

        AdminLogManager.Add(LogType.EventAnnounced, $"Event added / announced: {ToPrettyString(uid)}");

        // we don't want to send to players who aren't in game (i.e. in the lobby)
        Filter allPlayersInGame = Filter.Empty().AddWhere(GameTicker.UserHasJoinedGame);

        // Macrocosm edit start - announcer variation
        SoundSpecifier? soundSpecifier = null; // funky

        if (stationEvent.StartAudio is { } startAudio && Announcer.TryGetAnnouncerSound(startAudio, out soundSpecifier))
        {
            if (!paAnnouncements) // funky
                Audio.PlayGlobal(soundSpecifier, allPlayersInGame, true);
        }
        // Macrocosm edit end

        if (stationEvent.StartAnnouncement != null)
            ChatSystem.DispatchFilteredAnnouncement(allPlayersInGame, Loc.GetString(stationEvent.StartAnnouncement),
                playSound: paAnnouncements, announcementSound: paAnnouncements ? soundSpecifier : null, // funky
                colorOverride: stationEvent.StartAnnouncementColor);


    }

    /// <inheritdoc/>
    protected override void Started(EntityUid uid, T component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        if (!TryComp<StationEventComponent>(uid, out var stationEvent))
            return;

        AdminLogManager.Add(LogType.EventStarted, LogImpact.High, $"Event started: {ToPrettyString(uid)}");

        if (stationEvent.Duration != null)
        {
            var duration = stationEvent.MaxDuration == null
                ? stationEvent.Duration
                : TimeSpan.FromSeconds(RobustRandom.NextDouble(stationEvent.Duration.Value.TotalSeconds,
                    stationEvent.MaxDuration.Value.TotalSeconds));
            stationEvent.EndTime = Timing.CurTime + duration;
        }
    }

    /// <inheritdoc/>
    protected override void Ended(EntityUid uid, T component, GameRuleComponent gameRule, GameRuleEndedEvent args)
    {
        base.Ended(uid, component, gameRule, args);

        if (!TryComp<StationEventComponent>(uid, out var stationEvent))
            return;

        var paAnnouncements = _cfg.GetCVar(PAAnnouncementCVars.PAAnnouncements); // funky

        AdminLogManager.Add(LogType.EventStopped, $"Event ended: {ToPrettyString(uid)}");

        // we don't want to send to players who aren't in game (i.e. in the lobby)
        Filter allPlayersInGame = Filter.Empty().AddWhere(GameTicker.UserHasJoinedGame);

        // Macrocosm edit start - announcer variation
        SoundSpecifier? soundSpecifier = null; // funky

        if (stationEvent.EndAudio is { } endAudio && Announcer.TryGetAnnouncerSound(stationEvent.EndAudio.Value, out soundSpecifier))
        {
            if (!paAnnouncements) // funky
                Audio.PlayGlobal(soundSpecifier, allPlayersInGame, true);
        }
        // Macrocosm edit end
        if (stationEvent.EndAnnouncement != null)
            ChatSystem.DispatchFilteredAnnouncement(allPlayersInGame, Loc.GetString(stationEvent.EndAnnouncement),
                playSound: paAnnouncements, announcementSound: paAnnouncements ? soundSpecifier : null, // funky
                colorOverride: stationEvent.EndAnnouncementColor);


    }

    /// <summary>
    ///     Called every tick when this event is running.
    ///     Events are responsible for their own lifetime, so this handles starting and ending after time.
    /// </summary>
    /// <inheritdoc/>
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<StationEventComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var stationEvent, out var ruleData))
        {
            if (!GameTicker.IsGameRuleAdded(uid, ruleData))
                continue;

            if (!GameTicker.IsGameRuleActive(uid, ruleData) && !HasComp<DelayedStartRuleComponent>(uid))
            {
                GameTicker.StartGameRule(uid, ruleData);
            }
            else if (stationEvent.EndTime != null && Timing.CurTime >= stationEvent.EndTime && GameTicker.IsGameRuleActive(uid, ruleData))
            {
                GameTicker.EndGameRule(uid, ruleData);
            }
        }
    }
}
