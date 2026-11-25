using System.Linq;
using Content.Shared._Funkystation.SM.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Construction.Components;
using Content.Server.Singularity.Components;
using Content.Shared._Funkystation.SM;
using Content.Shared._Funkystation.SM.Events;
using Content.Shared._Funkystation.SM.Prototypes;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.CCVar;
using Content.Shared.Conveyor;
using Content.Shared.Radiation.Components;
using Content.Shared.SubFloor;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Funkystation.SM.EntitySystems;

public sealed partial class SupermatterSystem : SharedSupermatterSystem
{
    [Dependency] private AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private TransformSystem _transformSystem = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SupermatterLightningSystem _lightning = default!;
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private MapSystem _mapSystem = default!;
    [Dependency] private IRobustRandom _random = default!;


    private static readonly ISawmill Sawmill = Logger.GetSawmill("supermatterServer");
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SharedSupermatterComponent, AtmosDeviceUpdateEvent>(OnProcessSupermatter);
        SubscribeLocalEvent<SharedSupermatterComponent, MapInitEvent>(OnSupermatterMapInit);
        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypesReloaded);
        SubscribeLocalEvent<TransformComponent, AnchorStateChangedEvent>(OnAnchorChanged);
        SubscribeLocalEvent<FixturesComponent, MapInitEvent>(OnMapInit);

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
        sm.CurrentAlertLevel = _proto.Index(sm.BaseAlertState);
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
    /// Helper function that is a list of offsets
    /// </summary>
    private static readonly Vector2i[] AbsorptionOffsets =
    [
        new(-1, -1), new(0, -1), new(1, -1),
        new(-1,  0), new(0,  0), new(1,  0),
        new(-1,  1), new(0,  1), new(1,  1),
    ];
    /// <summary>
    /// Helper function to determin what is allowed to be close to the supermatter
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    private bool IsAllowedNearSupermatter(EntityUid uid)
    {
        return HasComp<SubFloorHideComponent>(uid) ||
               HasComp<ConveyorComponent>(uid);
    }
    /// <summary>
    /// Helper function to determin if an entity is close to the supermatter
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="supermatterUid"></param>
    /// <param name="supermatter"></param>
    /// <returns></returns>
    private bool IsAdjacentToSupermatter(EntityUid uid, out EntityUid supermatterUid,
        out SharedSupermatterComponent supermatter)
    {
        supermatterUid = EntityUid.Invalid;
        supermatter = default!;
        var xform = Transform(uid);

        if (xform.GridUid is not { } grid)
            return false;

        var tile = _transformSystem.GetGridOrMapTilePosition(uid);

        var query = EntityQueryEnumerator<SharedSupermatterComponent>();
        while (query.MoveNext(out var smUid, out var sm))
        {
            var smTile = _transformSystem.GetGridOrMapTilePosition(smUid);

            foreach (var offset in AbsorptionOffsets)
            {
                if (tile != smTile + offset)
                    continue;
                supermatterUid = smUid;
                supermatter = sm;
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Checks if a forbidden structure or anchored entity is near the SM
    /// </summary>
    /// <param name="uid"></param>
    private void CheckForbiddenStructure(EntityUid uid)
    {

        if (IsAllowedNearSupermatter(uid))
            return;

        if (!IsAdjacentToSupermatter(uid, out var smUid, out var sm))
            return;
        var recurse = !(TryComp<MachineComponent>(uid, out _) || TryComp<ComputerComponent>(uid, out var _) ||
                        TryComp<MachineFrameComponent>(uid, out var _));
        AttemptAshForbiddenStructure(smUid, uid, sm,recurseContainers: recurse);
    }

    /// <summary>
    /// Event handle for when an entity anchored, used to handle entites anchored near the SM
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="comp"></param>
    /// <param name="args"></param>
    private void OnAnchorChanged(EntityUid uid, TransformComponent comp, ref AnchorStateChangedEvent args)
    {
        if (!args.Anchored)
            return;

        if (!HasComp<FixturesComponent>(uid))
            return;

        CheckForbiddenStructure(uid);
    }

    /// <summary>
    /// Event handle for when an entity is spawned, used to handle anchored entities spawned near the SM
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="comp"></param>
    /// <param name="args"></param>
    private void OnMapInit(EntityUid uid, FixturesComponent comp, MapInitEvent args)
    {
        if (!Transform(uid).Anchored)
            return;

        CheckForbiddenStructure(uid);
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
        sm.LastAbsorbedGas.CopyFrom(sm.AbsorbedGas);
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

        var integrity = GetIntegrityPercent(sm.Integrity, sm.MaxIntegrity);

        if (integrity < sm.CurrentAlertLevel.LowerThreshold)
        {
            TransitionTo(uid, sm, sm.CurrentAlertLevel.LowerState);
        }

        if (integrity > sm.CurrentAlertLevel.UpperThreshold)
        {
            TransitionTo(uid, sm, sm.CurrentAlertLevel.UpperState);
        }

        sm.LastIntegrity = sm.Integrity;

        switch (sm.Delaminated)
        {
            case false when sm is { Delamming: false, Integrity: <= 0 }:
                BeginDelaminationCountdown(uid, sm);
                break;
            case true:
                return;
        }

        ComputeRadiation(sm);
        EmitRadiation(uid, sm);

        _lightning.ProcessLightning(uid, sm, args.dt);


        UpdateItemPull(uid, sm);

        ReleaseGas(uid, sm, args);

        sm.DelamBeganThisAtmos = false;
    }


    private void TransitionTo(
        EntityUid uid,
        SharedSupermatterComponent sm,
        ProtoId<SupermatterAlertLevelPrototype> next)
    {
        var oldLevel = sm.CurrentAlertLevel;
        var newLevel = _proto.Index(next);

        sm.CurrentAlertLevel = newLevel;

        var ev = new SupermatterAlertLevelChangedEvent(
            oldLevel,
            newLevel,
            sm.Integrity > sm.LastIntegrity);

        RaiseLocalEvent(uid, ref ev);
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
            _atmosphereSystem.Merge(sm.AbsorbedGas, absorbed);
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
        sm.Power = Math.Clamp(sm.Power, sm.MinPower, sm.MaxPower);
    }

    /// <summary>
    /// Computes the characteristics of the absorbed gas
    /// </summary>
    /// <param name="sm"></param>
    private void ComputeGasCharacteristics(SharedSupermatterComponent sm)
    {
        float stability = -sm.Power/sm.PowerStabilityScalar + sm.BaseStability + (sm.Integrity - sm.MaxIntegrity) / sm.IntegrityStabilityScalar;
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
        sm.Power += -sm.Power * sm.PowerStabilityDecayScalar * sm.Stability;
        sm.Power = Math.Clamp(sm.Power, sm.MinPower, sm.MaxPower);
    }

    /// <summary>
    /// Updates the Enthalpy
    /// </summary>
    /// <param name="sm"></param>
    private void ApplyEnthalpy( SharedSupermatterComponent sm)
    {
        var deltaEnergy = sm.Enthalpy * sm.JoulesPerEnthalpy; // MJ → joules
        sm.Power += sm.Enthalpy * (sm.AbsorbedGas.Temperature - sm.NeutralEnthalpyTemperature); // temperature - room temperature in Kelvin
        sm.Power = Math.Clamp(sm.Power, sm.MinPower, sm.MaxPower);
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
                    (sm.Enthalpy >= 0 ? sm.Enthalpy : -sm.Enthalpy, sm.Enthalpy >= 0 ? Gas.Plasma     : Gas.WaterVapor),
                    (sm.Conductivity >= 0 ? sm.Conductivity : -sm.Conductivity, sm.Conductivity >= 0 ? Gas.Frezon : Gas.Oxygen),
                    (stabilityScale >= 0 ? stabilityScale : -stabilityScale, stabilityScale >= 0 ? Gas.Nitrogen   : Gas.Tritium),
                };
                characteristics.Sort((a, b) => MathF.Abs(b.value).CompareTo(MathF.Abs(a.value)));
                for (var i = 0; i < count && i < characteristics.Count; i++)
                {
                    var (_, gas) = characteristics[i];
                    sm.AbsorbedGas.AdjustMoles((int)gas, amount);
                }

                sm.Power -= amount * count;
                sm.Power = Math.Clamp(sm.Power, sm.MinPower, sm.MaxPower);
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
                sm.Power = Math.Clamp(sm.Power, sm.MinPower, sm.MaxPower);
                sm.Reproduction += absorbedMoles;
                break;
            }
        }

    }

    /// <summary>
    /// Helper array with the offsets for around the SM
    /// </summary>
    private static readonly Vector2i[] ShardSpawnOffsets =
    [
        new(-1, -1), new(0, -1), new(1, -1),
        new(-1,  0),             new(1,  0),
        new(-1,  1), new(0,  1), new(1,  1),
    ];

    /// <summary>
    /// Helper function to spawn a shard on a random tile around the supermatter
    /// </summary>
    /// <param name="uid"></param>
    private void SpawnShard(EntityUid uid)
    {
        var xform = Transform(uid);

        if (xform.GridUid is not { } gridUid)
            return;

        if (!TryComp<MapGridComponent>(gridUid, out var gridComp))
            return;

        var centerTile = _transformSystem.GetGridOrMapTilePosition(uid);

        var possibleTiles = new List<Vector2i>();

        foreach (var offset in ShardSpawnOffsets)
        {
            var tile = centerTile + offset;

            if (_mapSystem.TryGetTileRef(gridUid, gridComp, tile, out _))
                possibleTiles.Add(tile);

        }

        if (possibleTiles.Count == 0)
            return;

        var chosenTile = _random.Pick(possibleTiles);

        var coords = _mapSystem.GridTileToLocal(gridUid, gridComp, chosenTile);

        Spawn("MaterialSupermatterShard1", coords);
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

            SpawnShard(uid);
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
            var safeTemp = sm.NeutralEnthalpyTemperature;
            if (sm.Enthalpy < 0)
            {
                safeTemp += (sm.Enthalpy / sm.MaxCharacteristic) * (sm.MinSafeTemp - sm.NeutralEnthalpyTemperature);
            }
            else
            {
                safeTemp += (sm.Enthalpy / sm.MaxCharacteristic) * (sm.MaxSafeTemp - sm.NeutralEnthalpyTemperature);
            }
            delta -= (gasTemp - safeTemp) / sm.TemperatureDamageScale;
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

    private bool HasCharacteristicTie(SharedSupermatterComponent sm)
    {
        var growth = MathF.Abs(sm.Growth);
        var conductivity = MathF.Abs(sm.Conductivity);
        var enthalpy = MathF.Abs(sm.Enthalpy);
        var stability = MathF.Abs(sm.Stability - sm.NeutralStability);

        var values = new[]
        {
            growth,
            conductivity,
            enthalpy,
            stability,
        };

        var max = values.Max();

        var ties = values.Count(v =>
            MathF.Abs(v - max) <= sm.TieTolerance);

        return ties > 1;
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
        sm.DelamLocked = false;
        sm.DelamBeganThisAtmos = true;
        sm.DelamCountdown = sm.DelamTimerDuration;
        if (sm.DelamCountdown <= 0)
            ResolveDelamination(uid, sm);
    }

    private void TickDelaminationCountdown(EntityUid uid, SharedSupermatterComponent sm, float dt)
    {
        sm.DelamCountdown -= dt;

        if (!sm.DelamLocked &&
            sm.DelamCountdown <= sm.DelamLockTime)
        {
            sm.PreferredDelamType = ChooseDelamType(uid, sm);
            sm.DelamLocked = true;
        }

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
        sm.DelamLocked = false;
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

        if (HasCharacteristicTie(sm))
        {
            return _cfg.GetCVar(CCVars.SupermatterDoCascadeDelam)
                ? DelamType.Cascade
                : DelamType.Explosion;
        }

        switch (GetDominantCharacteristic(sm))
        {
            case GasCharacteristicsType.Growth:
                return _cfg.GetCVar(CCVars.SupermatterDoSingulooseDelam)
                    ? DelamType.Singulo
                    : DelamType.Explosion;

            case GasCharacteristicsType.Conductivity:
                return _cfg.GetCVar(CCVars.SupermatterDoTeslooseDelam)
                    ? DelamType.Tesla
                    : DelamType.Explosion;

            case GasCharacteristicsType.Enthalpy:
                if (sm.Enthalpy > 0)
                    return DelamType.Explosion;

                return _cfg.GetCVar(CCVars.SupermatterDoCascadeDelam)
                    ? DelamType.Cascade
                    : DelamType.Explosion;

            case GasCharacteristicsType.Stability:
                if (sm.Stability < sm.NeutralStability)
                    return DelamType.Explosion;

                return _cfg.GetCVar(CCVars.SupermatterDoCascadeDelam)
                    ? DelamType.Cascade
                    : DelamType.Explosion;

            default:
                return DelamType.Explosion;
        }
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

        var stability = MathF.Abs(sm.Stability - sm.NeutralStability);
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
        var baseRadiation = sm.BaseRadiation + (sm.Power * sm.PowerRadsPercentage);
        var stabilityMultiplier = sm.MinimumRadsOutputPercentage + (sm.NeutralStability - sm.Stability)/sm.NeutralStability;
        // Floor so nominal stability still yields measurable output (radiation source + integration tests).
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

        var validTiles = new List<GasMixture>();

        foreach (var offset in AbsorptionOffsets)
        {
            var tile = centerTile + offset;

            var mixture = _atmosphereSystem.GetTileMixture(
                grid,
                args.Map,
                tile,
                excite: true);

            if (mixture != null)
                validTiles.Add(mixture);
        }

        if (validTiles.Count == 0)
            return;
        //clears Absorbed gas
        var gasToRelease = sm.AbsorbedGas.RemoveRatio(1f);
        //Gets the ratio of tiles to release gas on
        var ratio = 1f / validTiles.Count;
        //splits the gas so each tile gets the same ratio
        var perTile = gasToRelease.Clone();
        perTile.Multiply(ratio);
        //releases the gas
        foreach (var mixture in validTiles)
        {
            _atmosphereSystem.Merge(mixture, perTile.Clone());
        }
    }
}
