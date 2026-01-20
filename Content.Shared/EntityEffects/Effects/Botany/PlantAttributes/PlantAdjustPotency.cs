// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-License-Identifier: MIT

namespace Content.Shared.EntityEffects.Effects.Botany.PlantAttributes;

/// <summary>
///     Handles increase or decrease of plant potency.
/// </summary>
public sealed partial class PlantAdjustPotency : BasePlantAdjustAttribute<PlantAdjustPotency>
{
    public override string GuidebookAttributeName { get; set; } = "plant-attribute-potency";
}
