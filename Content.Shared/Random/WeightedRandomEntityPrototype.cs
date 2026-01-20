// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Random;

/// <summary>
/// Linter-friendly version of weightedRandom for Entity prototypes.
/// </summary>
[Prototype]
public sealed partial class WeightedRandomEntityPrototype : IWeightedRandomPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("weights", customTypeSerializer: typeof(PrototypeIdDictionarySerializer<float, EntityPrototype>))]
    public Dictionary<string, float> Weights { get; private set; } = new();
}
