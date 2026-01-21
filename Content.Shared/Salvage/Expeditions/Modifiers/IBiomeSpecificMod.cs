// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
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
