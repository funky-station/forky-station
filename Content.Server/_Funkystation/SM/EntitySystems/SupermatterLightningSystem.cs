using System.Linq;
using Content.Server.Lightning;
using Content.Server.Lightning.Components;
using Content.Shared._Funkystation.SM;
using Content.Shared._Funkystation.SM.Components;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.Random;

namespace Content.Server._Funkystation.SM.EntitySystems;
[Access( typeof(SharedSupermatterSystem))]
public sealed partial class SupermatterLightningSystem : SharedSupermatterLightningSystem
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private SharedTransformSystem _xform = default!;
    [Dependency] private LightningSystem _lightning = default!;
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private SharedBatterySystem _battery = default!;


    public void ProcessLightning(EntityUid uid, SharedSupermatterComponent sm, float atmosDeltaTime)
    {
        // No lightning unless conductivity is strong enough aka positive conductivity
        if (MathF.Abs(sm.Conductivity) < 0.001f)
            return;

        sm.LightningTimer -= atmosDeltaTime;

        if (sm.LightningTimer > 0f)
            return;

        FireLightning(uid, sm);

        sm.LightningTimer = ComputeInterval(sm);
    }

    private float ComputeInterval(SharedSupermatterComponent sm)
    {
        var c = MathF.Abs(sm.Conductivity);

        var interval = sm.LightningMaxInterval - c * sm.ConductivityScalar;

        return Math.Clamp(interval, sm.LightningMinInterval, sm.LightningMaxInterval);
    }

    private LightningTier GetLightningTier(SharedSupermatterComponent sm)
    {
        LightningTier? result = null;

        foreach (var tier in sm.LightningTiers)
        {
            if (sm.Power < tier.MinPower)
                continue;

            result = tier;
        }

        return result ?? sm.LightningTiers[0];
    }

    private void FireLightning(EntityUid uid, SharedSupermatterComponent sm)
    {

        var tier = GetLightningTier(sm);



        // Bolt count scales with power
        var bolts = tier.BoltCount;

        // Bolt energy
        var energy = tier.BoltEnergyMJ;

        // Lightning prototype (normal or black)
        var proto = sm.Conductivity >= 0 ? tier.Proto : "BlackLightning";

        // Arc depth scales with power
        var arcDepth = tier.ExtraBounces;

        if (sm.Conductivity > 0f)
            FirePositiveBolt(uid, sm.LightningRange, proto, energy, arcDepth, bolts);
        else
            FireNegativeBolt(uid, sm.LightningRange, proto, energy, bolts);

    }

    private void FirePositiveBolt(EntityUid sm, float range, string proto, float boltEnergy, int arcDepth, int boltCount)
    {
        _lightning.ShootRandomLightnings(sm,range , boltCount, proto, arcDepth, overrideEnergy: boltEnergy);


    }

    private void FireNegativeBolt(EntityUid sm, float range, string proto, float boltEnergy, int boltCount)
    {
        var xform = Transform(sm);
        var coords = xform.Coordinates;

        var targets = _lookup.GetEntitiesInRange<LightningTargetComponent>(coords, range).ToList();
        _random.Shuffle(targets);
        targets.Sort((x, y) => x.Comp.Priority.CompareTo(y.Comp.Priority));

        var shootedCount = 0;
        var count = -1;
        while(shootedCount < boltCount)
        {
            count++;

            if (count >= targets.Count) { break; }

            var curTarget = targets[count];
            curTarget.Comp.LightningExplode = false;
            if (!_random.Prob(curTarget.Comp.HitProbability)) //Chance to ignore target
                continue;

            _lightning.ShootLightning(targets[count].Owner, sm, proto, overrideEnergy: 0f);

            // Drain energy from target
            var drained = DrainEnergyFromTarget(curTarget, boltEnergy);

            // Feed it back into the SM
            var comp = Comp<SharedSupermatterComponent>(sm);
            comp.Power += drained;

            shootedCount++;
        }


    }

    private float DrainEnergyFromTarget(EntityUid target, float amount)
    {
        if (amount <= 0f || !TryComp<BatteryComponent>(target, out var battery))
            return 0f;

        var ent = (target, battery);
        var (available, maxCharge) = _battery.GetCharge(target);
        if (available <= 0f)
            return 0f;

        var take = MathF.Min(amount, available);
        return _battery.UseCharge(ent, take);
    }

}

