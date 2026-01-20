// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 chavonadelal
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Salvage.Expeditions.Modifiers;

[Prototype("salvageTemperatureMod")]
public sealed partial class SalvageTemperatureMod : IPrototype, IBiomeSpecificMod
{
    [IdDataField] public string ID { get; private set; } = default!;

    [DataField("desc")] public LocId Description { get; private set; } = string.Empty;

    /// <inheritdoc/>
    [DataField("cost")]
    public float Cost { get; private set; } = 0f;

    /// <inheritdoc/>
    [DataField]
    public List<ProtoId<SalvageBiomeModPrototype>>? Biomes { get; private set; } = null;

    /// <summary>
    /// Temperature in the planets air mix.
    /// </summary>
    [DataField("temperature")]
    public float Temperature = 293.15f;
}
