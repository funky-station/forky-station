using Content.Server.Chat.Systems;
using Content.Server.Station.Systems;
using Content.Shared.CriminalRecords;
using Content.Shared.CriminalRecords.Components;
using Content.Shared.CriminalRecords.Systems;
using Content.Shared.Random.Helpers;
using Content.Shared.Security;
using Content.Shared.StationRecords;
using Content.Shared.StationRecords.Systems;
using Robust.Shared.Random;
//<funkystation>
using Robust.Shared.Timing;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Radio;
//</funkystation>

namespace Content.Server.CriminalRecords.Systems;

public sealed partial class CriminalRecordsHackerSystem : SharedCriminalRecordsHackerSystem
{
    [Dependency] private ChatSystem _chat = default!;
    [Dependency] private CriminalRecordsSystem _criminalRecords = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private StationSystem _station = default!;
    [Dependency] private StationRecordsSystem _records = default!;
    [Dependency] private RadioSystem _radio = default!; //funkystation
    [Dependency] private IGameTiming _timing = default!; //funkystation

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CriminalRecordsHackerComponent, CriminalRecordsHackDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<CriminalRecordsHackerComponent, CriminalRecordHackStartEvent>(OnHackStart); //funkystation
    }
    private void OnDoAfter(Entity<CriminalRecordsHackerComponent> ent, ref CriminalRecordsHackDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled || args.Target == null)
            return;

        if (_station.GetOwningStation(ent) is not {} station)
            return;

        var reasons = ProtoMan.Index(ent.Comp.Reasons);
        foreach (var (key, record) in _records.GetRecordsOfType<CriminalRecord>(station))
        {
            var reason = _random.Pick(reasons);
            _criminalRecords.OverwriteStatus(new StationRecordKey(key, station), record, SecurityStatus.Wanted, reason);
            // no radio message since spam
            // no history since lazy and its easy to remove anyway
            // main damage with this is existing arrest warrants are lost and to anger beepsky
        }

        _chat.DispatchGlobalAnnouncement(Loc.GetString(ent.Comp.Announcement), playSound: true, colorOverride: Color.Red);

        // once is enough
        RemComp<CriminalRecordsHackerComponent>(ent);

        var ev = new CriminalRecordsHackedEvent(ent, args.Target.Value);
        RaiseLocalEvent(args.User, ref ev);
    }
    //funky station, warns sec when ninja begins hack
    private void OnHackStart(Entity<CriminalRecordsHackerComponent> ent, ref CriminalRecordHackStartEvent args)
    {
        if (_timing.CurTime >= ent.Comp.NextWarningTime) // prevents spam
        {
            var message = Loc.GetString("ninja-hack-wanted-warning");
            _radio.SendRadioMessage(args.Target, message, ProtoMan.Index<RadioChannelPrototype>(ent.Comp.SecurityChannel), args.Target, true, "Criminal Records Computer");
            ent.Comp.NextWarningTime = _timing.CurTime + ent.Comp.WarningCooldown;
        }
    }
}


/// <summary>
/// Raised on the user after hacking a criminal records console.
/// </summary>
[ByRefEvent]
public record struct CriminalRecordsHackedEvent(EntityUid User, EntityUid Target);
