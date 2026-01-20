// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-License-Identifier: MIT

using Robust.Shared.Noise;
using Robust.Shared.Serialization;

namespace Content.Shared.Parallax.Biomes.Layers;

/// <summary>
/// Dummy layer that specifies a marker to be replaced by external code.
/// For example if they wish to add their own layers at specific points across different templates.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class BiomeDummyLayer : IBiomeLayer
{
    [DataField("id", required: true)] public string ID = string.Empty;

    public FastNoiseLite Noise { get; } = new();
    public float Threshold { get; }
    public bool Invert { get; }
}
