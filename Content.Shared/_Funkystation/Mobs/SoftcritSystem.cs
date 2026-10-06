using Content.Shared._ES.Viewcone.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;

// I'm sure this is fine
namespace Content.Shared.Mobs.Systems;

public partial class MobStateSystem
{
    private const float SoftCritSpeedMultiplier = 0.4f;
    private const float SoftCritViewconeReduction = -110f;
    private const float HardCritViewconeReduction = -360f;

    private void InitializeSoftcrit()
    {
        SubscribeLocalEvent<MobStateComponent, RefreshMovementSpeedModifiersEvent>(OnSoftcritSpeedRefresh);
        SubscribeLocalEvent<MobStateComponent, ESViewconeGetAngleModifierEvent>(OnSoftcritAdjustViewcone);
        SubscribeLocalEvent<MobStateComponent, PullStartedMessage>(OnPullInteractionStateChanged);
        SubscribeLocalEvent<MobStateComponent, PullStoppedMessage>(OnPullInteractionStateChanged);
    }

    private void OnSoftcritSpeedRefresh(EntityUid uid, MobStateComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        if (component.CurrentState == MobState.SoftCritical)
        {
            args.ModifySpeed(SoftCritSpeedMultiplier, SoftCritSpeedMultiplier);
        }
    }

    private void OnPullInteractionStateChanged(EntityUid uid, MobStateComponent component, PullMessage args)
    {
        if (component.CurrentState == MobState.SoftCritical)
        {
            _blocker.UpdateCanMove(uid);
        }
    }

    private void OnSoftcritAdjustViewcone(EntityUid uid, MobStateComponent component, ref ESViewconeGetAngleModifierEvent args)
    {
        if (component.CurrentState == MobState.SoftCritical)
        {
            args.ModifyAngle(SoftCritViewconeReduction);
        }
        else if (component.CurrentState == MobState.HardCritical)
        {
            args.ModifyAngle(HardCritViewconeReduction);
        }
    }

    private bool ResolveStateFallback(MobState fromState, MobState toState, MobStateComponent component, out MobState resolvedState)
    {
        resolvedState = toState;

        if (toState != MobState.Critical)
            return false;

        if (component.AllowedStates.Contains(MobState.SoftCritical) && fromState == MobState.Alive)
        {
            resolvedState = MobState.SoftCritical;
            return true;
        }

        if (component.AllowedStates.Contains(MobState.HardCritical))
        {
            resolvedState = MobState.HardCritical;
            return true;
        }

        return false;
    }
}
