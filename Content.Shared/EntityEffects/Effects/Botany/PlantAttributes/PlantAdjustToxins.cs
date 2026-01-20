// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2025 pathetic meowmeow
// SPDX-License-Identifier: MIT

namespace Content.Shared.EntityEffects.Effects.Botany.PlantAttributes;

public sealed partial class PlantAdjustToxins : BasePlantAdjustAttribute<PlantAdjustToxins>
{
    public override string GuidebookAttributeName { get; set; } = "plant-attribute-toxins";

    public override bool GuidebookIsAttributePositive { get; protected set; } = false;
}

