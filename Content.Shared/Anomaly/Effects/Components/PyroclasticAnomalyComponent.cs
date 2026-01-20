// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 ThunderBear2006
// SPDX-License-Identifier: MIT

namespace Content.Shared.Anomaly.Effects.Components;

[RegisterComponent]
public sealed partial class PyroclasticAnomalyComponent : Component
{
    /// <summary>
    /// The maximum distance from which you can be ignited by the anomaly.
    /// </summary>
    [DataField("maximumIgnitionRadius")]
    public float MaximumIgnitionRadius = 5f;
}
