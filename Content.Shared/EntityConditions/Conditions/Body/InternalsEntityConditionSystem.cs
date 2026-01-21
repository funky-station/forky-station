// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-License-Identifier: MIT

using Content.Shared.Body.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityConditions.Conditions.Body;

/// <summary>
/// Returns true if this entity is using internals. False if they are not or cannot use internals.
/// </summary>
/// <inheritdoc cref="EntityConditionSystem{T, TCondition}"/>
public sealed partial class InternalsOnEntityConditionSystem : EntityConditionSystem<InternalsComponent, InternalsCondition>
{
    protected override void Condition(Entity<InternalsComponent> entity, ref EntityConditionEvent<InternalsCondition> args)
    {
        args.Result = entity.Comp.GasTankEntity != null;
    }
}

/// <inheritdoc cref="EntityCondition"/>
public sealed partial class InternalsCondition : EntityConditionBase<InternalsCondition>
{
    public override string EntityConditionGuidebookText(IPrototypeManager prototype) =>
        Loc.GetString("entity-condition-guidebook-internals", ("usingInternals", !Inverted));
}
