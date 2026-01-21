// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Server.Body.Components;
using Content.Shared.EntityConditions;
using Content.Shared.EntityConditions.Conditions.Body;

namespace Content.Server.EntityConditions.Conditions;

/// <summary>
/// Returns true if this entity has any of the listed metabolizer types.
/// </summary>
/// <inheritdoc cref="EntityConditionSystem{T, TCondition}"/>
public sealed partial class MetabolizerTypeEntityConditionSystem : EntityConditionSystem<MetabolizerComponent, MetabolizerTypeCondition>
{
    protected override void Condition(Entity<MetabolizerComponent> entity, ref EntityConditionEvent<MetabolizerTypeCondition> args)
    {
        if (entity.Comp.MetabolizerTypes == null)
            return;

        args.Result = entity.Comp.MetabolizerTypes.Overlaps(args.Condition.Type);
    }
}
