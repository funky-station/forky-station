//Funky begin - Announcement system is EE, different dependencies for forky also deprecated readonly will be removed in the future
using System.Linq;
using Content.Server.Radio.EntitySystems;
using Content.Server.Station.Systems;
using Content.Shared._Impstation.Service;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Server.Chat.Systems;

namespace Content.Server._Impstation.Service;

/// <summary>
///     System handling service job board.
///     TLDR: Console handles UI interactions,
///     all the actual data is stored by the station.
/// </summary>
public sealed partial class ServiceJobBoardSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private RadioSystem _radio = default!;
    [Dependency] private StationSystem _station = default!;
    [Dependency] private UserInterfaceSystem _ui = default!;
    [Dependency] private ChatSystem _chatSystem = default!;
    //Funky end - Announcement system is EE, different dependencies for forky

    private static readonly string AnnouncementName = "Station Event";
    private static readonly Color AnnouncementColor = Color.FromHex("#88BE14");

    public override void Initialize()
    {
        SubscribeLocalEvent<ServiceJobBoardConsoleComponent, BoundUIOpenedEvent>(OnBUIOpened);
        Subs.BuiEvents<ServiceJobBoardConsoleComponent>(ServiceJobBoardUiKey.Key,
            subs =>
            {
                subs.Event<ServiceJobBoardSelectMessage>(OnSelectMessage);
            });
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<ServiceJobsDataComponent>();
        var curTime = _timing.CurTime;
        //Funky begin - Announcement system is EE, adding a way for the board to reset
        while (query.MoveNext(out var uid, out var data))
        {
            var job = _prototypeManager.Index(data.ActiveJob);
            if (data.EndTime != null &&
                job != null &&
                data.EndTime.Value < curTime)
            {
                _chatSystem.DispatchStationAnnouncement(uid, Loc.GetString(job.StartAnnounce), AnnouncementName, colorOverride: AnnouncementColor);

                ResetJobs((uid, data));
            }
        }

    }

    private void ResetJobs(Entity<ServiceJobsDataComponent> ent)
    {
        ent.Comp.ActiveJob = null;
        ent.Comp.StationJobs = [];
        ent.Comp.EndTime = null;

        GetJobs(ent);
    }
    //Funky end - Announcement system is EE, adding a way for the board to reset


    private void OnBUIOpened(Entity<ServiceJobBoardConsoleComponent> ent, ref BoundUIOpenedEvent args)
    {
        if (args.UiKey is not ServiceJobBoardUiKey.Key)
            return;

        if (_station.GetOwningStation(ent.Owner) is not { } station ||
            !TryComp<ServiceJobsDataComponent>(station, out var jobData))
            return;

        UpdateUi(ent, (station, jobData));
    }

    private void OnSelectMessage(Entity<ServiceJobBoardConsoleComponent> ent, ref ServiceJobBoardSelectMessage args)
    {
        if (_station.GetOwningStation(ent) is not { } station ||
            !TryComp<ServiceJobsDataComponent>(station, out var jobData))
            return;

        if (!_prototypeManager.TryIndex<ServiceJobPrototype>(args.JobId, out var job))
            return;

        //Funky - Fixing warning
        if (!jobData.StationJobs.Contains(job))
            return;

        jobData.ActiveJob = job;
        jobData.EndTime = _timing.CurTime + job.Timer;

        var message = Loc.GetString(
            "service-job-console-select-announce",
            ("event", Loc.GetString(job.Name)),
            ("timer", job.Timer.ToString()));
        _radio.SendRadioMessage(ent, message, ent.Comp.AnnounceChannel, ent, false);

        // we need to update the state of all computers, not just the one in use
        var query = EntityQueryEnumerator<ServiceJobBoardConsoleComponent>();
        while (query.MoveNext(out var uid, out var console))
        {
            if (_station.GetOwningStation(ent.Owner) is not { } queryStation ||
                !TryComp<ServiceJobsDataComponent>(station, out var queryJobData))
                return;
            UpdateUi((uid, console), (queryStation, queryJobData));
        }
    }

    private void UpdateUi(Entity<ServiceJobBoardConsoleComponent> ent, Entity<ServiceJobsDataComponent> stationEnt)
    {
        //Funky - Added a countdown to the jobs end
        var state = new ServiceJobBoardConsoleState(
            GetJobs(stationEnt),
            stationEnt.Comp.ActiveJob,
            stationEnt.Comp.EndTime,
            _timing.CurTime);

        _ui.SetUiState(ent.Owner, ServiceJobBoardUiKey.Key, state);
    }

    /// <summary>
    ///     Gets the station's service jobs.
    ///     Will generate a new list if none exist.
    /// </summary>
    public List<ProtoId<ServiceJobPrototype>> GetJobs(Entity<ServiceJobsDataComponent> ent)
    {
        if (ent.Comp.StationJobs.Count == 0)
        {
            ent.Comp.StationJobs = [];

            //Funky begin - Fixing warning
            for (var i = 0; i < ent.Comp.MaxJobs; i++)
            {
                if (TryGetRandomJob(ent, out var job) && job != null)
                    ent.Comp.StationJobs.Add(job);
            }
            //Funky end - Fixing warning

        }
        return ent.Comp.StationJobs;
    }

    /// <summary>
    ///     Attempts to grab a random service job prototype, ignoring duplicates.
    ///     Fails if there are no jobs left.
    /// </summary>
    public bool TryGetRandomJob(Entity<ServiceJobsDataComponent> ent, out ServiceJobPrototype? job)
    {
        var allJobs = _prototypeManager.EnumeratePrototypes<ServiceJobPrototype>()
            .ToList();
        var filteredJobs = new List<ServiceJobPrototype>();
        foreach (var proto in allJobs)
        {
            if (ent.Comp.StationJobs.Any(j => j.Id == proto.ID))
                continue;
            filteredJobs.Add(proto);
        }

        if (filteredJobs.Count == 0)
        {
            job = null;
            return false;
        }

        job = _random.Pick(filteredJobs);
        return true;
    }
}
