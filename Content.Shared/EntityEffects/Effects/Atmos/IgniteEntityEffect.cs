// SPDX-FileCopyrightText: 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Database;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects.Atmos;

/// <summary>
/// See serverside system
/// </summary>
/// <inheritdoc cref="EntityEffect"/>
public sealed partial class Ignite : EntityEffectBase<Ignite>
{
    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("entity-effect-guidebook-ignite", ("chance", Probability));

    public override LogImpact? Impact => LogImpact.Medium;
}
