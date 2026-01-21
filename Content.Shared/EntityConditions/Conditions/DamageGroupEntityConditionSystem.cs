// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson <karakkaraz@gmail.com>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityConditions.Conditions;

/// <summary>
/// Returns true if this entity can take damage and if its damage of a given damage group is within a specified minimum and maximum.
/// </summary>
/// <inheritdoc cref="EntityConditionSystem{T, TCondition}"/>
public sealed partial class DamageGroupEntityConditionSystem : EntityConditionSystem<DamageableComponent, DamageGroupCondition>
{
    protected override void Condition(Entity<DamageableComponent> entity, ref EntityConditionEvent<DamageGroupCondition> args)
    {
        var value = entity.Comp.DamagePerGroup[args.Condition.DamageGroup];
        args.Result = value >= args.Condition.Min && value <= args.Condition.Max;
    }
}

/// <inheritdoc cref="EntityCondition"/>
public sealed partial class DamageGroupCondition : EntityConditionBase<DamageGroupCondition>
{
    [DataField]
    public FixedPoint2 Max = FixedPoint2.MaxValue;

    [DataField]
    public FixedPoint2 Min = FixedPoint2.Zero;

    [DataField(required: true)]
    public ProtoId<DamageGroupPrototype> DamageGroup;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype) =>
        Loc.GetString("entity-condition-guidebook-group-damage",
            ("max", Max == FixedPoint2.MaxValue ? int.MaxValue : Max.Float()),
            ("min", Min.Float()),
            ("type", prototype.Index(DamageGroup).LocalizedName));
}
