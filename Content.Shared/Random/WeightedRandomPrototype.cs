// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Random;

/// <summary>
/// Generic random weighting dataset to use.
/// </summary>
[Prototype]
public sealed partial class WeightedRandomPrototype : IWeightedRandomPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("weights")]
    public Dictionary<string, float> Weights { get; private set; } = new();
}
