// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Salvage.Expeditions.Modifiers;

public interface IBiomeSpecificMod : ISalvageMod
{
    /// <summary>
    /// Whitelist for biomes. If null then any biome is allowed.
    /// </summary>
    List<ProtoId<SalvageBiomeModPrototype>>? Biomes { get; }
}
