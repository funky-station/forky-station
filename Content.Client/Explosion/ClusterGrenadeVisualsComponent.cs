// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-License-Identifier: MIT

namespace Content.Client.Explosion;

[RegisterComponent]
[Access(typeof(ClusterGrenadeVisualizerSystem))]
public sealed partial class ClusterGrenadeVisualsComponent : Component
{
    [DataField("state")]
    public string? State;
}
