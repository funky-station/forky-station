// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 drakewill-CRL
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 forthbridge
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Random;

/// <summary>
///     Random weighting dataset for solutions, able to specify reagents quantity.
/// </summary>
[Prototype("RandomPlantMutationList")]
public sealed partial class RandomPlantMutationListPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = default!;

    /// <summary>
    ///     List of RandomFills that can be picked from.
    /// </summary>
    [DataField("mutations", required: true, serverOnly: true)]
    public List<RandomPlantMutation> mutations = new();
}
