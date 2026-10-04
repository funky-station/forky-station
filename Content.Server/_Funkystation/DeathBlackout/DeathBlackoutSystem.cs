using Content.Shared._Funkystation.CCVar;
using Content.Shared._Funkystation.DeathBlackout;
using Content.Shared.Ghost.Systems;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.DeathBlackout;

public sealed partial class DeathBlackoutSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private IGameTiming _timing = null!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);
        SubscribeLocalEvent<DeathBlackoutComponent, GhostAttemptEvent>(OnGhostAttempt);
    }

    /// <summary>
    /// starts the blackout when a mob with a mind dies
    /// </summary>
    private void OnMobStateChanged(MobStateChangedEvent args)
    {
        var ent = args.Target;

        if (args.NewMobState != MobState.Dead)
        {
            if (args.OldMobState == MobState.Dead)
                RemComp<DeathBlackoutComponent>(ent);

            return;
        }

        if (!_cfg.GetCVar(DeathBlackoutCVars.Enabled))
            return;

        if (!TryComp<MindContainerComponent>(ent, out var mindContainer) || !mindContainer.HasMind)
            return;

        var blackout = EnsureComp<DeathBlackoutComponent>(ent);
        blackout.EndTime = _timing.CurTime + TimeSpan.FromSeconds(_cfg.GetCVar(DeathBlackoutCVars.Duration));
        Dirty(ent, blackout);
    }

    // no ghosting until the blackout is over. you're COMPROMISING my CINEMATIC VISION
    private void OnGhostAttempt(Entity<DeathBlackoutComponent> ent, ref GhostAttemptEvent args)
    {
        if (!_cfg.GetCVar(DeathBlackoutCVars.Enabled))
            return;

        if (_timing.CurTime < ent.Comp.EndTime)
            args.Cancelled = true;
    }
}
