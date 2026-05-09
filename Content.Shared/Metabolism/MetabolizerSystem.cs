using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Body.Events;
using Content.Shared.Body;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityConditions;
using Content.Shared.EntityConditions.Conditions;
using Content.Shared.EntityConditions.Conditions.Body;
using Content.Shared.EntityEffects;
using Content.Shared.EntityEffects.Effects.Body;
using Content.Shared.EntityEffects.Effects.Solution;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Collections;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Metabolism;

/// <inheritdoc/>
public sealed class MetabolizerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly SharedEntityConditionsSystem _entityConditions = default!;
    [Dependency] private readonly SharedEntityEffectsSystem _entityEffects = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;
    [Dependency] private readonly INetManager _net = default!;

    private EntityQuery<OrganComponent> _organQuery;
    private EntityQuery<SolutionContainerManagerComponent> _solutionQuery;

    public override void Initialize()
    {
        base.Initialize();

        _organQuery = GetEntityQuery<OrganComponent>();
        _solutionQuery = GetEntityQuery<SolutionContainerManagerComponent>();

        SubscribeLocalEvent<MetabolizerComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<MetabolizerComponent, BodyRelayedEvent<ApplyMetabolicMultiplierEvent>>(OnApplyMetabolicMultiplier);
    }

    private void OnMapInit(Entity<MetabolizerComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.NextUpdate = _gameTiming.CurTime + ent.Comp.AdjustedUpdateInterval;
    }

    private void OnApplyMetabolicMultiplier(Entity<MetabolizerComponent> ent, ref BodyRelayedEvent<ApplyMetabolicMultiplierEvent> args)
    {
        ent.Comp.UpdateIntervalMultiplier = args.Args.Multiplier;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // We only do this on the server to prevent the client from reshuffling metabolism during prediction.
        // Should just be replaced with predicted random.
        if (_net.IsClient)
            return;

        var metabolizers = new ValueList<(EntityUid Uid, MetabolizerComponent Component)>(Count<MetabolizerComponent>());
        var query = EntityQueryEnumerator<MetabolizerComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            metabolizers.Add((uid, comp));
        }

        foreach (var (uid, metab) in metabolizers)
        {
            // Only update as frequently as it should
            if (_gameTiming.CurTime < metab.NextUpdate)
                continue;

            metab.NextUpdate += metab.AdjustedUpdateInterval;
            TryMetabolize((uid, metab));
        }
    }

    /// <summary>
    /// Updates the metabolic rate multiplier for a given entity,
    /// raising both <see cref="GetMetabolicMultiplierEvent"/> to determine what the multiplier is and <see cref="ApplyMetabolicMultiplierEvent"/> to update relevant components.
    /// </summary>
    /// <param name="uid"></param>
    public void UpdateMetabolicMultiplier(EntityUid uid)
    {
        var getEv = new GetMetabolicMultiplierEvent();
        RaiseLocalEvent(uid, ref getEv);

        var applyEv = new ApplyMetabolicMultiplierEvent(getEv.Multiplier);
        RaiseLocalEvent(uid, ref applyEv);
    }

    private bool LookupSolution(
        Entity<MetabolizerComponent, OrganComponent?, SolutionContainerManagerComponent?> ent,
        MetabolismSolutionEntry solutionData,
        bool lookupTransfer,
        [NotNullWhen(true)] out Solution? solution,
        [NotNullWhen(true)] out Entity<SolutionComponent>? solutionEntity,
        [NotNullWhen(true)] out EntityUid? solutionOwner
    )
    {
        solution = null;
        solutionEntity = null;
        solutionOwner = null;

        var solutionName = lookupTransfer ? solutionData.TransferSolutionName : solutionData.SolutionName;

        if (solutionName is null)
            return false;

        if (lookupTransfer ? solutionData.TransferSolutionOnBody : solutionData.SolutionOnBody)
        {
            if (ent.Comp2?.Body is { } body)
            {
                if (!_solutionQuery.TryComp(body, out var bodySolution))
                    return false;

                solutionOwner = body;
                return _solutionContainerSystem.TryGetSolution((body, bodySolution), solutionName, out solutionEntity, out solution);
            }
        }
        else
        {
            if (!_solutionQuery.Resolve(ent, ref ent.Comp3, logMissing: false))
                return false;

            solutionOwner = ent;
            return _solutionContainerSystem.TryGetSolution((ent, ent), solutionName, out solutionEntity, out solution);
        }

        return false;
    }

    private void TryMetabolizeStage(
    Entity<MetabolizerComponent, OrganComponent?, SolutionContainerManagerComponent?> ent,
    ProtoId<MetabolismStagePrototype> stage)
{
    if (!ent.Comp1.Solutions.TryGetValue(stage, out var solutionData))
        return;

    if (!LookupSolution(ent, solutionData, false, out var solution, out var solutionEntity, out var solutionOwner))
        return;

    if (solution.Contents.Count == 0)
        return;

    LookupSolution(ent, solutionData, true, out var transferSolution, out var transferSolutionEntity, out _);

    // Copy the contents list so we don't modify it while iterating.
    var list = solution.Contents.ToList();

    // Collect reagents excluded from this metabolism pass (e.g. blood reagents).
    var ev = new MetabolismExclusionEvent();
    RaiseLocalEvent(solutionOwner.Value, ref ev);

    // Randomise order to avoid processing-order artefacts.
    _random.Shuffle(list);

    var isDead = _mobStateSystem.IsDead(solutionOwner.Value);

    var reagents = 0;
    foreach (var (reagent, quantity) in list)
    {
        if (!_prototypeManager.TryIndex<ReagentPrototype>(reagent.Prototype, out var proto))
            continue;

        if (ev.Reagents.Contains(reagent))
            continue;

        // Determine the administration route for this specific reagent instance.
        var route = GetAdministrationRoute(reagent);

        // Pick the most specific metabolism entry available.
        if (!TryGetMetabolismEntry(proto, stage, route, out var entry))
        {
            // No metabolism defined: transfer the reagent to the next stage.
            var mostToTransfer = FixedPoint2.Clamp(solutionData.TransferRate, 0, quantity);

            if (transferSolution is not null)
            {
                solution.RemoveReagent(reagent, mostToTransfer);
                transferSolution.AddReagent(reagent, mostToTransfer * solutionData.TransferEfficacy);
            }
            else
            {
                solution.RemoveReagent(reagent, FixedPoint2.New(1));
            }

            continue;
        }

        var rate = solutionData.MetabolizeAll ? quantity : entry.MetabolismRate;
        var mostToRemove = FixedPoint2.Clamp(rate, 0, quantity);

        if (reagents >= ent.Comp1.MaxReagentsProcessable)
            return;

        var scale = (float) mostToRemove;
        if (!solutionData.MetabolizeAll)
            scale /= (float) rate;

        if (isDead && !proto.WorksOnTheDead)
            continue;

        var actualEntity = ent.Comp2?.Body ?? solutionOwner.Value;

        foreach (var effect in entry.Effects)
        {
            if (scale < effect.MinScale)
                continue;

            if (effect.Probability < 1.0f && !_random.Prob(effect.Probability))
                continue;

            if (effect.Conditions != null
                && !CanMetabolizeEffect(actualEntity, ent, solutionEntity.Value, effect.Conditions))
                continue;

            ApplyEffect(effect);
        }

        void ApplyEffect(EntityEffect effect)
        {
            switch (effect)
            {
                case ModifyLungGas:
                    _entityEffects.ApplyEffect(ent, effect, scale);
                    break;
                case AdjustReagent:
                    _entityEffects.ApplyEffect(solutionEntity.Value, effect, scale);
                    break;
                default:
                    _entityEffects.ApplyEffect(actualEntity, effect, scale);
                    break;
            }
        }

        if (mostToRemove > FixedPoint2.Zero)
        {
            solution.RemoveReagent(reagent, mostToRemove);
            reagents += 1;

            if (transferSolution is not null && entry.Metabolites is not null)
            {
                foreach (var (metabolite, ratio) in entry.Metabolites)
                {
                    transferSolution.AddReagent(metabolite, mostToRemove * ratio);
                }
            }
        }
    }

    _solutionContainerSystem.UpdateChemicals(solutionEntity.Value);

    if (transferSolutionEntity is not null)
        _solutionContainerSystem.UpdateChemicals(transferSolutionEntity.Value);
}

/// <summary>
/// Returns the <see cref="ReagentAdministrationRoute"/> stored in a reagent's data list,
/// or <see cref="ReagentAdministrationRoute.Unspecified"/> if none is present.
/// </summary>
private static ReagentAdministrationRoute GetAdministrationRoute(ReagentId reagent)
{
    if (reagent.Data == null)
        return ReagentAdministrationRoute.Unspecified;

    foreach (var data in reagent.Data)
    {
        if (data is AdministrationRouteData routeData)
            return routeData.Route;
    }

    return ReagentAdministrationRoute.Unspecified;
}

/// <summary>
/// Attempts to find the most appropriate <see cref="ReagentEffectsEntry"/> for the given
/// reagent prototype, metabolism stage, and administration route.
/// Route-specific metabolisms take priority over the prototype's default metabolisms.
/// </summary>
/// <param name="proto">The reagent prototype to search.</param>
/// <param name="stage">The metabolism stage being processed.</param>
/// <param name="route">The administration route of the reagent instance.</param>
/// <param name="entry">The resolved effects entry, if any.</param>
/// <returns>
/// <see langword="true"/> if an entry was found in either route-specific or default metabolisms.
/// </returns>
private static bool TryGetMetabolismEntry(
    ReagentPrototype proto,
    ProtoId<MetabolismStagePrototype> stage,
    ReagentAdministrationRoute route,
    [NotNullWhen(true)] out ReagentEffectsEntry? entry)
{
    entry = null;

    // Route-specific metabolisms take priority.
    if (route != ReagentAdministrationRoute.Unspecified
        && proto.RouteMetabolisms != null
        && proto.RouteMetabolisms.TryGetValue(route, out var routeMetabolisms)
        && routeMetabolisms.Metabolisms.TryGetValue(stage, out entry))
    {
        return true;
    }

    // Fall back to the default metabolisms.
    return proto.Metabolisms?.Metabolisms.TryGetValue(stage, out entry) == true;
}

    private void TryMetabolize(Entity<MetabolizerComponent, OrganComponent?, SolutionContainerManagerComponent?> ent)
    {
        _organQuery.Resolve(ent, ref ent.Comp2, logMissing: false);

        foreach (var stage in ent.Comp1.Stages)
        {
            TryMetabolizeStage(ent, stage);
        }
    }

    /// <summary>
    /// Public API to check if a certain metabolism effect can be applied to an entity.
    /// TODO: With metabolism refactor make this logic smarter and unhardcode the old hardcoding entity effects used to have for metabolism!
    /// </summary>
    /// <param name="body">The body metabolizing the effects</param>
    /// <param name="organ">The organ doing the metabolizing</param>
    /// <param name="solution">The solution we are metabolizing from</param>
    /// <param name="conditions">The conditions that need to be met to metabolize</param>
    /// <returns>True if we can metabolize! False if we cannot!</returns>
    public bool CanMetabolizeEffect(EntityUid body, EntityUid organ, Entity<SolutionComponent> solution, EntityCondition[] conditions)
    {
        foreach (var condition in conditions)
        {
            switch (condition)
            {
                // Need specific handling of specific conditions since Metabolism is funny like that.
                // TODO: MetabolizerTypes should be handled well before this stage by metabolism itself.
                case MetabolizerTypeCondition:
                    if (_entityConditions.TryCondition(organ, condition))
                        continue;
                    break;
                case ReagentCondition:
                    if (_entityConditions.TryCondition(solution, condition))
                        continue;
                    break;
                default:
                    if (_entityConditions.TryCondition(body, condition))
                        continue;
                    break;
            }

            return false;
        }

        return true;
    }
}

