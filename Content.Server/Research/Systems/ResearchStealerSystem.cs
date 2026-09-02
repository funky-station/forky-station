using Content.Shared.Research.Components;
using Content.Shared.Research.Systems;
using Robust.Shared.Random;
//<funky change>
using Robust.Shared.Timing;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Radio;
using Robust.Shared.Prototypes;
//</funky change>

namespace Content.Server.Research.Systems;

public sealed partial class ResearchStealerSystem : SharedResearchStealerSystem
{
    [Dependency] private SharedResearchSystem _research = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private RadioSystem _radio = default!; //funky
    [Dependency] private IGameTiming _timing = default!; // funky
    [Dependency] private IPrototypeManager _proto = default!; //funky

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ResearchStealerComponent, ResearchStealDoAfterEvent>(OnDoAfter);
    }

    private void OnDoAfter(EntityUid uid, ResearchStealerComponent comp, ResearchStealDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled || args.Target == null)
            return;

        var target = args.Target.Value;

        if (!TryComp<TechnologyDatabaseComponent>(target, out var database))
            return;

        var ev = new ResearchStolenEvent(uid, target, new());
        var count = _random.Next(comp.MinToSteal, comp.MaxToSteal + 1);
        for (var i = 0; i < count; i++)
        {
            if (database.UnlockedTechnologies.Count == 0)
                break;

            var toRemove = _random.Pick(database.UnlockedTechnologies);
            if (_research.TryRemoveTechnology((target, database), toRemove))
                ev.Techs.Add(toRemove);
        }

        // funky change, warns security when ninja attempts to hack comms console
        if (_timing.CurTime >= comp.NextWarningTime) // prevents spam
        {
            var message = Loc.GetString("ninja-steal-research-warning");
            _radio.SendRadioMessage(target, message, _proto.Index<RadioChannelPrototype>(comp.ScienceChannel), target, true, "Research Server");
            comp.NextWarningTime = _timing.CurTime + comp.WarningCooldown;
        }
        RaiseLocalEvent(uid, ref ev);

        args.Handled = true;
    }
}

/// <summary>
/// Event raised on the user when research is stolen from a RND server.
/// Techs contains every technology id researched.
/// </summary>
[ByRefEvent]
public record struct ResearchStolenEvent(EntityUid Used, EntityUid Target, List<string> Techs);
