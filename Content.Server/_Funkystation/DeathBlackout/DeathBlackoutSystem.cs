using Content.Shared._Funkystation.CCVar;
using Content.Shared._Funkystation.DeathBlackout;
using Content.Shared.Body;
using Content.Shared.Body.Components;
using Content.Shared.Ghost.Systems;
using Content.Shared.Gibbing;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.DeathBlackout;

public sealed partial class DeathBlackoutSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private IGameTiming _timing = null!;

    /// <summary>
    /// starts the blackout when a mob with a mind dies
    /// </summary>
    [SubscribeLocalEvent]
    private void OnMobStateChanged(MobStateChangedEvent args)
    {
        var ent = args.Target;

        if (args.NewMobState != MobState.Dead)
        {
            if (args.OldMobState == MobState.Dead)
                RemComp<DeathBlackoutComponent>(ent);

            return;
        }

        if (!TryComp<MindContainerComponent>(ent, out var mindContainer) || !mindContainer.HasMind)
            return;

        ApplyDeathBlackout(ent);
    }

    [SubscribeLocalEvent]
    private void OnBeingGibbed(Entity<BrainComponent> ent, ref BodyRelayedEvent<BeingGibbedEvent> relayedEvent)
    {
        if (!TryComp<MindContainerComponent>(relayedEvent.Body, out var mindContainer) || !mindContainer.HasMind)
            return;

        if (TryComp<MobStateComponent>(relayedEvent.Body, out var mobStateComponent) &&
            mobStateComponent.CurrentState == MobState.Dead)
            return;

        ApplyDeathBlackout(ent);
    }

    private void ApplyDeathBlackout(EntityUid ent)
    {
        if (!_cfg.GetCVar(DeathBlackoutCVars.Enabled))
            return;

        var blackout = EnsureComp<DeathBlackoutComponent>(ent);
        blackout.EndTime = _timing.CurTime + TimeSpan.FromSeconds(_cfg.GetCVar(DeathBlackoutCVars.Duration));
        Dirty(ent, blackout);
    }

    // no ghosting until the blackout is over. you're COMPROMISING my CINEMATIC VISION
    [SubscribeLocalEvent]
    private void OnGhostAttempt(Entity<DeathBlackoutComponent> ent, ref GhostAttemptEvent args)
    {
        if (!_cfg.GetCVar(DeathBlackoutCVars.Enabled))
            return;

        if (_timing.CurTime < ent.Comp.EndTime)
            args.Cancelled = true;
    }
}
