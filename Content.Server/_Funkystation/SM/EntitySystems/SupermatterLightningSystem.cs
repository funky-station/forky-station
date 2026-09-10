using System.Linq;
using Content.Server.Lightning;
using Content.Server.Lightning.Components;
using Content.Shared._Funkystation.SM;
using Content.Shared._Funkystation.SM.Components;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server._Funkystation.SM.EntitySystems;

public sealed partial class SupermatterLightningSystem : SharedSupermatterLightningSystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;
    [Dependency] private readonly LightningSystem _lightning = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedBatterySystem _battery = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SharedSupermatterComponent>();

        while (query.MoveNext(out var uid, out var sm))
        {
            // No lightning unless conductivity is strong enough
            if (MathF.Abs(sm.Conductivity) < 1f)
                continue;

            sm.LightningTimer -= frameTime;

            if (sm.LightningTimer > 0f)
                continue;

            FireLightning(uid, sm);

            sm.LightningTimer = ComputeInterval(sm);
        }
    }

    private float ComputeInterval(SharedSupermatterComponent sm)
    {
        var c = MathF.Abs(sm.Conductivity);

        // Base: 10s - 0.1s per conductivity
        var interval = 10f - 0.1f * c;

        // Minimum interval
        return MathF.Max(interval, 0.5f);
    }

    private void FireLightning(EntityUid uid, SharedSupermatterComponent sm)
    {


        // Bolt count scales with power
        var bolts = (int)Math.Clamp((int)(sm.Power / sm.PowerPerBolt), 1, sm.MaxBolts);

        // Bolt energy
        var energy = ComputeBoltEnergy(sm);

        // Lightning prototype (normal or black)
        var proto = sm.Conductivity >= 0 ? "Lightning" : "BlackLightning";

        // Arc depth scales with conductivity
        var arcDepth = (int)MathF.Max(1, MathF.Abs(sm.Conductivity));

        if (sm.Conductivity >= 1f)
            FirePositiveBolt(uid, sm.LightningRange, proto, energy, arcDepth, bolts);
        else
            FireNegativeBolt(uid, sm.LightningRange, proto, energy, bolts);

    }


    private float ComputeBoltEnergy(SharedSupermatterComponent sm)
    {
        // Simple example: sqrt scaling feels good
        return MathF.Sqrt(sm.Power) * MathF.Abs(sm.Conductivity);
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

        int shootedCount = 0;
        int count = -1;
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
        var available = _battery.GetCharge(ent);
        if (available <= 0f)
            return 0f;

        var take = MathF.Min(amount, available);
        return _battery.UseCharge(ent, take);
    }

}

