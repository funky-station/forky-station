using Content.IntegrationTests.Tests.Interaction;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Medical;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Prototypes;
using Content.Shared._Funkystation.CCVar; // funky
using Content.Shared.Body.Components; // funky
using Content.Shared.Chemistry.EntitySystems; // funky
using Content.Shared.Chemistry.Reagent; // funky

namespace Content.IntegrationTests.Tests.Medical;

/// <summary>
/// Tests for defibrilators.
/// </summary>
[TestOf(typeof(DefibrillatorComponent))]
public sealed class DefibrillatorTest : InteractionTest
{
    // We need two hands to use a defbrillator.
    protected override string PlayerPrototype => "MobHuman";

    private static readonly EntProtoId DefibrillatorProtoId = "Defibrillator";
    private static readonly EntProtoId TargetProtoId = "MobHuman";
    private static readonly ProtoId<DamageTypePrototype> BluntDamageTypeId = "Blunt";
    private static readonly ProtoId<ReagentPrototype> EpinephrineReagentId = "Epinephrine"; // funky, needs adrenaline to defib

    /// <summary>
    /// Kills a target mob, heals them and then revives them with a defibrillator.
    /// </summary>
    [Test]
    public async Task KillAndReviveTest()
    {
        var damageableSystem = SEntMan.System<DamageableSystem>();
        var mobThresholdsSystem = SEntMan.System<MobThresholdSystem>();
        var mobStateSystem = SEntMan.System<MobStateSystem>(); // funky
        var solutionContainerSystem = SEntMan.System<SharedSolutionContainerSystem>(); // funky

        // set revive chance cvar to 100
        await Server.WaitPost(() => Server.CfgMan.SetCVar(DefibrillatorCVars.ReviveChance, 1f));

        // Don't let the player and target suffocate.
        await AddAtmosphere();

        await SpawnTarget(TargetProtoId);

        var targetMobState = Comp<MobStateComponent>();
        var targetDamageable = Comp<DamageableComponent>();
        var targetBloodstream = Comp<BloodstreamComponent>(); // funky

        // Check that the target has no damage and is not crit or dead.
        Assert.Multiple(() =>
        {
            Assert.That(targetMobState.CurrentState, Is.EqualTo(MobState.Alive), "Target mob was not alive when spawned.");
            Assert.That(damageableSystem.GetTotalDamage(STarget!.Value), Is.EqualTo(FixedPoint2.Zero), "Target mob was damaged when spawned.");
        });

        // Get the damage needed to kill or crit the target.
        var critThreshold = mobThresholdsSystem.GetThresholdForState(STarget.Value, MobState.Critical);
        var deathThreshold = mobThresholdsSystem.GetThresholdForState(STarget.Value, MobState.Dead);
        var critDamage = new DamageSpecifier(ProtoMan.Index(BluntDamageTypeId), (critThreshold + deathThreshold) / 2);
        var deathDamage = new DamageSpecifier(ProtoMan.Index(BluntDamageTypeId), deathThreshold);

        // Kill the target by applying blunt damage.
        await Server.WaitPost(() => damageableSystem.SetDamage((STarget.Value, targetDamageable), deathDamage));
        await RunTicks(3);

        // Check that the target is dead.
        Assert.Multiple(() =>
        {
            Assert.That(targetMobState.CurrentState, Is.EqualTo(MobState.Dead), "Target mob did not die from deadly damage amount.");
            Assert.That(damageableSystem.GetTotalDamage(STarget!.Value), Is.EqualTo(deathThreshold), "Target mob had the wrong total damage amount after being killed.");
        });

        // Spawn a defib and activate it.
        var defib = await PlaceInHands(DefibrillatorProtoId, enableToggleable: true);
        var cooldown = Comp<DefibrillatorComponent>(defib).ZapDelay;

        // Wait for the cooldown.
        await RunSeconds((float)cooldown.TotalSeconds);

        // ZAP!
        await Interact();

        // Check that the target is still dead since it is over the crit threshold.
        // And it should have taken some extra damage.
        Assert.Multiple(() =>
        {
            Assert.That(targetMobState.CurrentState, Is.EqualTo(MobState.Dead), "Target mob was revived despite being over the death damage threshold.");
            Assert.That(damageableSystem.GetTotalDamage(STarget!.Value), Is.GreaterThan(deathThreshold), "Target mob did not take damage from being defibrillated.");
        });

        // add epi to the bloodstream
        await Server.WaitPost(() =>
        {
            var bloodSolution = targetBloodstream.BloodSolution;
            if (solutionContainerSystem.ResolveSolution(STarget.Value, targetBloodstream.BloodSolutionName, ref bloodSolution))
            {
                solutionContainerSystem.TryAddReagent(bloodSolution.Value, EpinephrineReagentId, 10, out _);
            }
        });

        // Set the damage halfway between the crit and death thresholds so that the target can be revived.
        await Server.WaitPost(() => damageableSystem.SetDamage((STarget.Value, targetDamageable), critDamage));
        await RunTicks(3);

        // Check that the target is still dead.
        Assert.That(targetMobState.CurrentState, Is.EqualTo(MobState.Dead), "Target mob revived on its own.");

        // ZAP!
        await RunSeconds((float)cooldown.TotalSeconds);
        await Interact();

        // The target should be revived into a critical state, softcrit or hardcrit
        Assert.Multiple(() =>
        {
            Assert.That(mobStateSystem.IsCritical(STarget.Value, targetMobState), Is.True, "Target mob was not in critical state after being defibrillated.");
            Assert.That(mobStateSystem.IsDead(STarget.Value, targetMobState), Is.False, "Target mob was still dead after being defibrillated.");
        });
    }
}
