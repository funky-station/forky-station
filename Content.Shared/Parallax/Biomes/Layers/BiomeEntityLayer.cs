// SPDX-FileCopyrightText: 2023, 2025 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023, 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Maps;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Parallax.Biomes.Layers;

[Serializable, NetSerializable]
public sealed partial class BiomeEntityLayer : IBiomeWorldLayer
{
    /// <inheritdoc/>
    [DataField]
    public List<ProtoId<ContentTileDefinition>> AllowedTiles { get; private set; } = new();

    [DataField("noise")] public FastNoiseLite Noise { get; private set; } = new(0);

    /// <inheritdoc/>
    [DataField("threshold")]
    public float Threshold { get; private set; } = 0.5f;

    /// <inheritdoc/>
    [DataField("invert")] public bool Invert { get; private set; } = false;

    [DataField(required: true)]
    public List<EntProtoId> Entities = new();
}
