using Content.Server._Funkystation.Objectives.Components;
using Content.Server.Cargo.Systems;
using Content.Shared.Interaction;
using Content.Shared.Mind.Components;
using Content.Shared.Objectives.Components;
using Content.Shared.Objectives.Systems;
using JetBrains.Annotations;
using Robust.Shared.Containers;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server._Funkystation.Objectives.Systems;

/// <summary>
/// The objective system for scorethief
/// </summary>
// Large portions of this were taken from https://github.com/funky-station/forky-station/blob/22a547c7c8aa1f0f6ac5f8c3e9941f2dfc25bd17/Content.Server/Objectives/Systems/StealConditionSystem.cs
[UsedImplicitly]
public sealed partial class ScoreThiefConditionSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private MetaDataSystem _metaData = default!;
    [Dependency] private SharedObjectivesSystem _objectives = default!;
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private SharedInteractionSystem _interaction = default!;
    [Dependency] private IEntityManager _entManager = default!;
    [Dependency] private EntityQuery<ContainerManagerComponent> _containerQuery = default!;
    [Dependency] private EntityQuery<ScoreThiefPriceModifierComponent> _modifierQuery = default!;
    [Dependency] private EntityQuery<MindContainerComponent> _mindQuery = default!;

   public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ScoreThiefConditionComponent, ObjectiveAssignedEvent>(OnAssigned);
        SubscribeLocalEvent<ScoreThiefConditionComponent, ObjectiveAfterAssignEvent>(OnAfterAssign);
        SubscribeLocalEvent<ScoreThiefConditionComponent, ObjectiveGetProgressEvent>(OnGetProgress);

    }

    private void OnAssigned(Entity<ScoreThiefConditionComponent> condition, ref ObjectiveAssignedEvent args)
    {
        condition.Comp.TargetScore = _random.Next(
            (condition.Comp.AmountSpesos - condition.Comp.AmountSpesosVariance)/condition.Comp.AmountSpesosVarianceInterval,
            (condition.Comp.AmountSpesos + condition.Comp.AmountSpesosVariance)/condition.Comp.AmountSpesosVarianceInterval
            ) * condition.Comp.AmountSpesosVarianceInterval;
    }

    private void OnAfterAssign(Entity<ScoreThiefConditionComponent> condition, ref ObjectiveAfterAssignEvent args)
    {
        string rsiState;
        switch(condition.Comp.TargetScore)
        {
            case 1:
                rsiState = "cash";
                break;
            case <= 10:
                rsiState = "cash_10";
                break;
            case <= 100:
                rsiState = "cash_100";
                break;
            case <= 500:
                rsiState = "cash_500";
                break;
            case <= 1000:
                rsiState = "cash_1000";
                break;
            case <= 5000:
                rsiState = "cash_5000";
                break;
            case <= 10000:
                rsiState = "cash_10000";
                break;
            case <= 25000:
                rsiState = "cash_25000";
                break;
            case <= 50000:
                rsiState = "cash_50000";
                break;
            case <= 100000:
                rsiState = "cash_100000";
                break;
            default:
                rsiState = "cash_1000000";
                break;
        }
        var sprite = new SpriteSpecifier.Rsi(new ResPath("/Textures/Objects/Economy/cash.rsi"), rsiState);

        _metaData.SetEntityName(condition.Owner,
            Loc.GetString("scorethief-objective-title-one") + " " + condition.Comp.TargetScore + " " + Loc.GetString("scorethief-objective-title-two"),
            args.Meta);
        _objectives.SetIcon(condition.Owner, sprite, args.Objective);
    }

    private void OnGetProgress(Entity<ScoreThiefConditionComponent> condition, ref ObjectiveGetProgressEvent args)
    {
        condition.Comp.CurrentScore = 0;
        var checkPlayer = true;

        // Check steal areas
        if (condition.Comp.CheckStealAreas)
        {
            var areasQuery = AllEntityQuery<StealAreaComponent, TransformComponent>();
            while (areasQuery.MoveNext(out var uid, out var area, out var xform))
            {
                if (!area.Owners.Contains(args.MindId))
                    continue;

                HashSet<Entity<TransformComponent>> nearestEnts = new();

                _lookup.GetEntitiesInRange<TransformComponent>(xform.Coordinates, area.Range, nearestEnts);
                foreach (var ent in nearestEnts)
                {
                    if (!_interaction.InRangeUnobstructed((uid, xform), (ent, ent.Comp), range: area.Range))
                        continue;

                    condition.Comp.CurrentScore += GetValue(ent, out _, area: true);
                    if (ent == args.Mind.CurrentEntity)
                    {
                        checkPlayer = false;
                    }
                }
            }
        }

        // Check thief's inventory
        if (args.Mind.CurrentEntity != null && checkPlayer)
        {
            condition.Comp.CurrentScore += GetValue(args.Mind.CurrentEntity.Value, out _, self: true);
        }

        args.Progress = Math.Clamp((float)condition.Comp.CurrentScore / condition.Comp.TargetScore, 0f, 1f);
        _metaData.SetEntityDescription(condition.Owner, Math.Clamp((int)((float)condition.Comp.CurrentScore/condition.Comp.TargetScore*100), 0, 100) + "%");
    }

    /// Get the price of an item (not including contained items) taking into account ScoreThiefPriceModifierComponent
    public int GetValue(EntityUid entity, out string? reason, bool area = false, bool self = false)
    {
        reason = null;
        var priceSystem = _entManager.System<PricingSystem>();
        double multiplier = 1;
        if (_modifierQuery.TryGetComponent(entity, out var modifier))
        {
            multiplier = modifier.Multiplier;
            if (modifier.Reason != null)
            {
                reason = Loc.GetString(modifier.Reason);
            }
        }

        if (_mindQuery.HasComp(entity))
        {
            if (!self)
            {
                reason = Loc.GetString("scorethief-modifier-alive");
                return 0;
            }
            multiplier = 0;
        }

        var price = 0;

        price = (int)(priceSystem.GetPrice(entity, false)*multiplier);

        if (_containerQuery.TryComp(entity, out var containerManager) && !area)
        {
            foreach (var container in containerManager.Containers.Values)
            {
                foreach (var ent in container.ContainedEntities)
                {
                    price += GetValue(ent, out _);
                }
            }
        }

        return price;
    }

    private int GetValueRecursive(ContainerManagerComponent currentManager)
    {
        var value = 0;
        // recursively check each container
        // checks inventory, bag, implants, etc.
        foreach (var container in currentManager.Containers.Values)
        {
            foreach (var entity in container.ContainedEntities)
            {
                value += GetValue(entity, out _);
            }
        }
        return value;
    }
}
