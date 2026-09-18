using Content.Shared._Funkystation.SM.Components;
using Content.Server._Funkystation.SM.Events;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Singularity.Components;
using Content.Shared._Funkystation.SM;
using Content.Shared._Funkystation.SM.Components;
using Content.Shared._Funkystation.SM.Prototypes;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.CCVar;
using Content.Shared.Radiation.Components;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
namespace Content.Server._Funkystation.SM.EntitySystems;

public sealed partial class SupermatterSystem : SharedSupermatterSystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;


    private static readonly ISawmill Sawmill = Logger.GetSawmill("supermatterServer");
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SharedSupermatterComponent, AtmosDeviceUpdateEvent>(OnProcessSupermatter);
        SubscribeLocalEvent<SharedSupermatterComponent, MapInitEvent>(OnSupermatterMapInit);
        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypesReloaded);

        LoadGasCharacteristics();
    }

    /// <summary>
    /// Prototype sets <see cref="RadiationSourceComponent"/> intensity to 0 until the first atmos tick;
    /// sync immediately so radiation hazards and tests see non-zero output without waiting for <see cref="AtmosDeviceUpdateEvent"/>.
    /// </summary>
    private void OnSupermatterMapInit(EntityUid uid, SharedSupermatterComponent sm, MapInitEvent args)
    {
        if (sm.Delaminated)
            return;
        LoadGasCharacteristics();
        ComputeRadiation(sm);
        EmitRadiation(uid, sm);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SharedSupermatterComponent>();
        while (query.MoveNext(out var uid, out var sm))
        {
            if (!sm.Delamming || sm.Delaminated)
                continue;

            TickDelaminationCountdown(uid, sm, frameTime);
        }
    }

    /// <summary>
    /// checks if the GasCharacteristicsPrototype was modified
    /// </summary>
    /// <param name="ev"></param>
    private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
    {
        if (ev.WasModified<GasCharacteristicsPrototype>())
            LoadGasCharacteristics();
    }

    /// <summary>
    /// Loads the Gas characteristics from yml
    /// </summary>
    private void LoadGasCharacteristics()
    {
        var newTable = new Dictionary<Gas, GasCharacteristics>();

        foreach (var proto in _proto.EnumeratePrototypes<GasCharacteristicsPrototype>())
        {
            if (!Enum.TryParse<Gas>(proto.ID, out var gas))
                continue;

            newTable[gas] = new GasCharacteristics(
                proto.Stability,
                proto.Growth,
                proto.Conductivity,
                proto.Enthalpy
            );
        }

        foreach (var sm in EntityQuery<SharedSupermatterComponent>())
        {
            sm.GasTable = newTable;
        }
    }

    /// <summary>
    /// Process logic for each supermatter.
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="sm"></param>
    /// <param name="args"></param>
    private void OnProcessSupermatter(EntityUid uid, SharedSupermatterComponent sm, AtmosDeviceUpdateEvent args)
    {
        sm.AbsorbedGas.Clear();
        AbsorbGas(uid, sm, args);
        if (sm.Activated)
            ApplyPowerPool(sm);
        ComputeGasCharacteristics(sm);
        ApplyPowerMultipliers(sm);
        ApplyStability(sm);
        ApplyEnthalpy(sm);
        ApplyGrowth(sm);

        if (!sm.Activated)
        {
            sm.Power = 0f;
            sm.PowerPool = 0f;
        }

        UpdateReproductionAndShards(uid, sm);
        sm.CurrentConductivity = sm.Conductivity;

        if (sm.Delaminated)
            return;

        // Integrity already zero (e.g. test injection) — begin delam before UpdateIntegrity can heal the crystal.
        if (!sm.Delaminated && !sm.Delamming && sm.Integrity <= 0)
            BeginDelaminationCountdown(uid, sm);

        UpdateIntegrity(uid, sm);

        if (!sm.Delaminated && !sm.Delamming && sm.Integrity <= 0)
            BeginDelaminationCountdown(uid, sm);

        if (sm.Delaminated)
            return;

        ComputeRadiation(sm);
        EmitRadiation(uid, sm);

        UpdateItemPull(uid, sm);

        ReleaseGas(uid, sm, args);

        sm.DelamBeganThisAtmos = false;
    }

    /// <summary>
    /// TG wiki: powered crystal pulls loose items — implemented via a weak <see cref="GravityWellComponent"/>.
    /// </summary>
    private void UpdateItemPull(EntityUid uid, SharedSupermatterComponent sm)
    {
        if (sm.Delaminated || sm.Power < 1f)
        {
            // Avoid GravityWellSystem pulsing with MaxRange 0 (engine asserts positive range).
            RemComp<GravityWellComponent>(uid);
            return;
        }

        var grav = EnsureComp<GravityWellComponent>(uid);
        grav.MaxRange = Math.Clamp(sm.Power / 1000f, sm.MinGravRange, sm.MaxGravRange);
        grav.BaseRadialAcceleration = sm.GravAcceleration;
    }

    /// <summary>
    /// Helper function that is a list of offsets
    /// </summary>
    private static readonly Vector2i[] AbsorptionOffsets =
    {
        new(-1, -1), new(0, -1), new(1, -1),
        new(-1,  0), new(0,  0), new(1,  0),
        new(-1,  1), new(0,  1), new(1,  1)
    };

    /// <summary>
    /// Absorbs gas in a 3 x 3 area with the Supermatter at the center
    /// </summary>
    /// <param name="smUid"></param>
    /// <param name="sm"></param>
    /// <param name="args"></param>
    private void AbsorbGas(EntityUid smUid, SharedSupermatterComponent sm, AtmosDeviceUpdateEvent args)
    {
        sm.CountVacuumTiles = 0;
        if (args.Grid is not {} grid)
            return;
        var centerTile = _transformSystem.GetGridTilePositionOrDefault(smUid);

        foreach (var offset in AbsorptionOffsets)
        {
            var tile = centerTile + offset;

            var mixture = _atmosphereSystem.GetTileMixture(grid, args.Map, tile, excite: true);

            if (mixture == null)
            {
                sm.CountVacuumTiles++;
                continue;
            }

            var pressure = mixture.Pressure;
            if (pressure < sm.VacuumThreshold)
                sm.CountVacuumTiles++;

            if(pressure <= 0)
                continue;

            var absorbed = mixture.RemoveRatio(sm.RatioPerTile);
            foreach (var (gas, moles) in absorbed)
            {
                sm.AbsorbedGas.AdjustMoles(gas, moles);
            }
            sm.AbsorbedGas.Temperature = absorbed.Temperature;
        }
    }

    /// <summary>
    /// Adds the power from when an entity is ashed to the SM
    /// </summary>
    /// <param name="sm"></param>
    private void ApplyPowerPool(SharedSupermatterComponent sm)
    {
        if (sm.PowerPool <= 0.1f)
            return;

        var gained = sm.PowerPool * 0.10f; // 10%
        sm.Power += gained;
        sm.PowerPool -= gained;
    }

    /// <summary>
    /// Computes the characteristics of the absorbed gas
    /// </summary>
    /// <param name="sm"></param>
    private void ComputeGasCharacteristics(SharedSupermatterComponent sm)
    {
        float stability = sm.BaseStability + sm.Integrity / sm.IntegrityStabilityScalar;
        float growth = sm.BaseGrowth;
        float conductivity = sm.BaseConductivity;
        float enthalpy = sm.BaseEnthalpy;

        foreach (var (gas, moles) in sm.AbsorbedGas )
        {
            if (moles <= 0f)
                continue;

            if (!sm.GasTable.TryGetValue(gas, out var ch))
                continue;

            stability    += (moles * ch.Stability) / sm.GasCharacteristicScalar;
            growth       += (moles * ch.Growth) / sm.GasCharacteristicScalar;
            conductivity += (moles * ch.Conductivity) / sm.GasCharacteristicScalar;
            enthalpy     += (moles * ch.Enthalpy) / sm.GasCharacteristicScalar;
        }

        // Per-tick totals from absorbed gas (base + table contribution), not cumulative across ticks.
        sm.Stability = MathF.Min(stability, sm.NeutralStability);
        sm.Growth = growth;
        sm.Conductivity = conductivity;
        sm.Enthalpy = enthalpy;
    }

    /// <summary>
    /// Characteristic Multiplication by Power
    /// </summary>
    /// <param name="sm"></param>
    private void ApplyPowerMultipliers(SharedSupermatterComponent sm)
    {
        var multiplier = 1f + sm.Power / sm.PowerScalingFactor;
        sm.Growth *= multiplier;
        sm.Conductivity *= multiplier;
        sm.Enthalpy *= multiplier;
    }

    /// <summary>
    /// Updates the stability
    /// </summary>
    /// <param name="sm"></param>
    private void ApplyStability(SharedSupermatterComponent sm)
    {
        var stabilityEffectScale = (sm.NeutralStability - sm.Stability) / sm.NeutralStability;

        sm.Growth       *= stabilityEffectScale;
        sm.Conductivity *= stabilityEffectScale;
        sm.Enthalpy     *= stabilityEffectScale;
        sm.Power += sm.Power * -sm.StabilityPowerDrainScale * sm.Stability;
        if (sm.Power <= 0f)
            sm.Power = 0f;
    }

    /// <summary>
    /// Updates the Enthalpy
    /// </summary>
    /// <param name="sm"></param>
    private void ApplyEnthalpy( SharedSupermatterComponent sm)
    {
        var deltaEnergy = sm.Enthalpy * 1_000_000f; // MJ → joules
        sm.Power += sm.Enthalpy * (sm.AbsorbedGas.Temperature - sm.NeutralEnthalpyTemperature); // temperature - room temperature in Kelvin
        if (sm.Power <= 0f)
            sm.Power = 0f;
        _atmosphereSystem.AddHeat(sm.AbsorbedGas, deltaEnergy);
    }

    /// <summary>
    /// Updates the growth
    /// </summary>
    /// <param name="sm"></param>
    private void ApplyGrowth(SharedSupermatterComponent sm)
    {
        switch (sm.Growth)
        {
            //Negative Growth
            case < 0f:
            {
                var amount = -sm.Growth;
                var count = (int)MathF.Floor((sm.Power + sm.PowerPerGasPacket) / sm.PowerPerGasPacket);
                if (count < 1)
                    count = 1;

                var stabilityScale = sm.Stability / sm.NeutralStability;
                var characteristics = new List<(float value, Gas gas)>
                {
                    (sm.Growth,        Gas.Ammonia),
                    (sm.Enthalpy >= 0 ? sm.Enthalpy : -sm.Enthalpy, sm.Enthalpy >= 0 ? Gas.Plasma     : Gas.Frezon),
                    (sm.Conductivity >= 0 ? sm.Conductivity : -sm.Conductivity, sm.Conductivity >= 0 ? Gas.WaterVapor : Gas.Oxygen),
                    (stabilityScale >= 0 ? stabilityScale : -stabilityScale, stabilityScale >= 0 ? Gas.Nitrogen   : Gas.Tritium),
                };
                characteristics.Sort((a, b) => MathF.Abs(b.value).CompareTo(MathF.Abs(a.value)));
                for (var i = 0; i < count && i < characteristics.Count; i++)
                {
                    var (_, gas) = characteristics[i];
                    sm.AbsorbedGas.AdjustMoles((int)gas, amount);
                }

                sm.Power -= amount * count;
                if (sm.Power <= 0f)
                    sm.Power = 0f;
                return;
            }
            //Positive growth
            case > 0f:
            {
                var fraction = sm.Growth / sm.GrowthAbsorptionScale;
                if (fraction <= 0f)
                    break;

                if (fraction > 1f)
                    fraction = 1f;

                var absorbed = sm.AbsorbedGas.RemoveRatio(fraction);
                _atmosphereSystem.Merge(sm.AbsorbedGas, absorbed);

                var absorbedMoles = absorbed.TotalMoles;

                if (absorbedMoles <= 0f)
                    break;

                sm.Power += Math.Abs(absorbedMoles);
                sm.Reproduction += absorbedMoles;
                break;
            }
        }

    }

    /// <summary>
    /// Updates the reproduction and creates a shard when reaching the threshold
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="sm"></param>
    private void UpdateReproductionAndShards(EntityUid uid, SharedSupermatterComponent sm)
    {
        if (HasComp<SupermatterShardComponent>(uid))
            return;

        sm.Reproduction *= sm.ReproductionDecay;

        sm.ReproductionProgress += sm.Reproduction;

        while (sm.ReproductionProgress >= sm.ReproductionThreshold)
        {
            sm.ReproductionProgress -= sm.ReproductionThreshold;

            var coords = Transform(uid).Coordinates;
            Spawn("SupermatterShard", coords);
        }
    }

    /// <summary>
    /// Updates the integrity of the supermatter crystal
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="sm"></param>
    private void UpdateIntegrity(EntityUid uid, SharedSupermatterComponent sm)
    {
        if (sm.Delaminated)
            return;

        var delta = 0f;

        if (sm.Activated)
        {
            delta += sm.Stability;
            delta -= sm.Power / sm.PowerDamageScale;

            if (sm.Power > sm.VacuumDamageMinPower)
            {
                delta -= sm.CountVacuumTiles * sm.VacuumDamagePerTile;
            }

            var gasTemp = sm.AbsorbedGas.Temperature;
            float neutralTemp = Atmospherics.T20C;
            neutralTemp += (sm.Stability - sm.NeutralStability) * sm.Enthalpy;
            var tempDelta = ((neutralTemp - gasTemp) / sm.TemperatureDamageScale);
            if (sm.Enthalpy < 0)
            {
                tempDelta = -1;
            }
            delta += tempDelta;
        }

        if (sm.AbsorptionHealingPool > sm.AbsorptionHealingCost)
        {
            delta += sm.AbsorptionHealing;
            sm.AbsorptionHealingPool -= sm.AbsorptionHealingCost;
        }

        sm.Integrity += Math.Clamp(delta, -sm.IntegrityChangeCap, sm.IntegrityChangeCap);
        sm.Integrity = Math.Clamp(sm.Integrity, 0f, sm.MaxIntegrity);

        // TG wiki: during the final countdown the crystal can recover if integrity heals back above zero.
        if (sm.Delamming && sm.Integrity > 0f && !sm.DelamBeganThisAtmos)
        {
            sm.Delamming = false;
            sm.DelamCountdown = 0f;
        }
    }

    /// <summary>
    /// Checks if the supermatters intregrity has hit 0
    /// and raises an event if it has which trigger the delamination
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="sm"></param>
    private void BeginDelaminationCountdown(EntityUid uid, SharedSupermatterComponent sm)
    {
        sm.PreferredDelamType = ChooseDelamType(uid, sm);
        sm.Delamming = true;
        sm.DelamBeganThisAtmos = true;
        sm.DelamCountdown = sm.DelamTimerDuration;
        if (sm.DelamCountdown <= 0)
            ResolveDelamination(uid, sm);
    }

    private void TickDelaminationCountdown(EntityUid uid, SharedSupermatterComponent sm, float dt)
    {
        sm.DelamCountdown -= dt;
        if (sm.DelamCountdown <= 0)
            ResolveDelamination(uid, sm);
    }

    private void ResolveDelamination(EntityUid uid, SharedSupermatterComponent sm)
    {
        var dominant = GetDominantCharacteristic(sm);
        var ev = new SupermatterDelaminationEvent(uid, dominant, sm.PreferredDelamType);
        RaiseLocalEvent(uid, ref ev);
        sm.Delaminated = true;
        sm.Delamming = false;
        sm.DelamCountdown = 0;
        _audio.PlayPvs(sm.AmbientSoundDelamming, uid);
    }

    /// <summary>
    /// Delamination priority (later wiki list entries beat earlier): cascade &gt; singularity &gt; tesla &gt; default explosion.
    /// </summary>
    public DelamType ChooseDelamType(EntityUid uid, SharedSupermatterComponent sm)
    {
        if (_cfg.GetCVar(CCVars.SupermatterDoForceDelam))
        {
            var forced = _cfg.GetCVar(CCVars.SupermatterForcedDelamType);
            if (forced >= 0 && forced <= (byte)DelamType.Cascade)
                return (DelamType)(byte)forced;
            return DelamType.Explosion;
        }

        var minNob = _cfg.GetCVar(CCVars.SupermatterCascadeNobMinFraction);
        var cascadeMoles = _cfg.GetCVar(CCVars.SupermatterCascadeMinAbsorbedMoles);
        if (_cfg.GetCVar(CCVars.SupermatterDoCascadeDelam) &&
            AbsorbedMixQualifiesForResonanceCascade(sm.AbsorbedGas, cascadeMoles, minNob))
            return DelamType.Cascade;

        var singuloNeed = _cfg.GetCVar(CCVars.SupermatterSinguloAbsorbedMolesThreshold) *
                          _cfg.GetCVar(CCVars.SupermatterSingulooseMolesModifier);
        if (_cfg.GetCVar(CCVars.SupermatterDoSingulooseDelam) && sm.AbsorbedGas.TotalMoles > singuloNeed)
            return DelamType.Singulo;

        var powerNeed = _cfg.GetCVar(CCVars.SupermatterPowerPenaltyThreshold) *
                        _cfg.GetCVar(CCVars.SupermatterTesloosePowerModifier);
        if (_cfg.GetCVar(CCVars.SupermatterDoTeslooseDelam) && sm.Power > powerNeed)
            return DelamType.Tesla;

        return DelamType.Explosion;
    }

    /// <summary>
    /// Resonance cascade: both nob gases above fraction threshold in the absorbed mix, total absorbed moles above minimum.
    /// </summary>
    public static bool AbsorbedMixQualifiesForResonanceCascade(GasMixture mix, float minTotalMoles, float minNobFraction)
    {
        var total = mix.TotalMoles;
        if (total < minTotalMoles)
            return false;

        var anti = mix.GetMoles(Gas.AntiNoblium);
        var hyper = mix.GetMoles(Gas.HyperNoblium);
        if (anti <= 0f || hyper <= 0f)
            return false;

        return anti / total > minNobFraction && hyper / total > minNobFraction;
    }

    /// <summary>
    /// Percent crystal remaining (0–100), for portable test parity with historical damage-based display.
    /// </summary>
    public static float GetIntegrityPercent(SharedSupermatterComponent sm) =>
        GetIntegrityPercent(sm.Integrity, sm.MaxIntegrity);

    public static float GetIntegrityPercent(float integrity, float maxIntegrity)
    {
        if (maxIntegrity <= 0f)
            return 0f;
        return integrity / maxIntegrity * 100f;
    }

    /// <summary>
    /// Gets the dominant Characteristics for delamination
    /// </summary>
    /// <param name="sm"></param>
    /// <returns></returns>
    private GasCharacteristicsType GetDominantCharacteristic(SharedSupermatterComponent sm)
    {
        // Start with Growth as the default
        var dominant = GasCharacteristicsType.Growth;
        var max = MathF.Abs(sm.Growth);

        var conductivity = MathF.Abs(sm.Conductivity);
        if (conductivity > max)
        {
            max = conductivity;
            dominant = GasCharacteristicsType.Conductivity;
        }

        var enthalpy = MathF.Abs(sm.Enthalpy);
        if (enthalpy > max)
        {
            max = enthalpy;
            dominant = GasCharacteristicsType.Enthalpy;
        }

        var stability = MathF.Abs(sm.Stability / sm.NeutralStability);
        if (stability > max)
        {
            dominant = GasCharacteristicsType.Stability;
        }

        return dominant;
    }

    /// <summary>
    /// Computes the radiation output of the Supermatter based on power and stability
    /// </summary>
    /// <param name="sm"></param>
    private void ComputeRadiation(SharedSupermatterComponent sm)
    {
        var baseRadiation = sm.BaseRadiation + (sm.Power * sm.PowerPercentage);
        var stabilityMultiplier = (sm.NeutralStability - sm.Stability) / sm.NeutralStability;
        // Floor so nominal stability still yields measurable output (radiation source + integration tests).
        stabilityMultiplier = MathF.Max(0.05f, stabilityMultiplier);
        sm.CurrentRadiation = baseRadiation * stabilityMultiplier;
    }

    /// <summary>
    /// Updates the RadiationSourceComponent with the current radiation intensity of the supermatter
    /// </summary>
    /// <param name="smUid"></param>
    /// <param name="sm"></param>
    private void EmitRadiation(EntityUid smUid, SharedSupermatterComponent sm)
    {
        var rad = EnsureComp<RadiationSourceComponent>(smUid);
        rad.Intensity = sm.CurrentRadiation;
    }

    /// <summary>
    /// Releases the gases the sm absorbed and produced
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="sm"></param>
    /// <param name="args"></param>
    private void ReleaseGas(EntityUid uid,SharedSupermatterComponent sm, AtmosDeviceUpdateEvent args)
    {
        if (args.Grid is not {} grid)
            return;
        var centerTile = _transformSystem.GetGridTilePositionOrDefault(uid);

        var mixture = _atmosphereSystem.GetTileMixture(grid, args.Map, centerTile, excite: true);
        if (mixture == null)
            return;

        _atmosphereSystem.Merge(mixture, sm.AbsorbedGas);
    }



}
