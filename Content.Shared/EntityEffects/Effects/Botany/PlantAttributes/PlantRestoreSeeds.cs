// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects.Botany.PlantAttributes;

/// <summary>
///     Handles restoral of seeds on a plant.
/// </summary>
public sealed partial class PlantRestoreSeeds : EntityEffectBase<PlantRestoreSeeds>
{
    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("entity-effect-guidebook-plant-seeds-add", ("chance", Probability));
}
