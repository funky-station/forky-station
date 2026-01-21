// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-License-Identifier: MIT

using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityConditions.Conditions.Tags;

/// <summary>
/// Returns true if this entity has the listed tag.
/// </summary>
/// <inheritdoc cref="EntityConditionSystem{T, TCondition}"/>
public sealed partial class HasTagEntityConditionSystem : EntityConditionSystem<TagComponent, TagCondition>
{
    [Dependency] private readonly TagSystem _tag = default!;

    protected override void Condition(Entity<TagComponent> entity, ref EntityConditionEvent<TagCondition> args)
    {
        args.Result = _tag.HasTag(entity.Comp, args.Condition.Tag);
    }
}

/// <inheritdoc cref="EntityCondition"/>
public sealed partial class TagCondition : EntityConditionBase<TagCondition>
{
    [DataField(required: true)]
    public ProtoId<TagPrototype> Tag;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype) =>
        Loc.GetString("entity-condition-guidebook-has-tag", ("tag", Tag), ("invert", Inverted));
}
