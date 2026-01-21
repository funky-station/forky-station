// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.EntityEffects.Effects.Botany.PlantAttributes;

/// <summary>
///     Handles increase or decrease of plant potency.
/// </summary>
public sealed partial class PlantAdjustPotency : BasePlantAdjustAttribute<PlantAdjustPotency>
{
    public override string GuidebookAttributeName { get; set; } = "plant-attribute-potency";
}
