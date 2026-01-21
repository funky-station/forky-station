// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects;

/// <summary>
/// A brief summary of the effect.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T, TEffect}"/>
public sealed partial class TemplateEntityEffectSystem : EntityEffectSystem<MetaDataComponent, Template>
{
    protected override void Effect(Entity<MetaDataComponent> entity, ref EntityEffectEvent<Template> args)
    {
        // Effect goes here.
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class Template : EntityEffectBase<Template>
{
    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) => null;
}
