// SPDX-FileCopyrightText: 2023, 2025 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023, 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Decals;
using Content.Shared.Maps;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Parallax.Biomes.Layers;

[Serializable, NetSerializable]
public sealed partial class BiomeDecalLayer : IBiomeWorldLayer
{
    /// <inheritdoc/>
    [DataField]
    public List<ProtoId<ContentTileDefinition>> AllowedTiles { get; private set; } = new();

    /// <summary>
    /// Divide each tile up by this amount.
    /// </summary>
    [DataField("divisions")]
    public float Divisions = 1f;

    [DataField("noise")]
    public FastNoiseLite Noise { get; private set; } = new(0);

    /// <inheritdoc/>
    [DataField("threshold")]
    public float Threshold { get; private set; } = 0.8f;

    /// <inheritdoc/>
    [DataField("invert")] public bool Invert { get; private set; } = false;

    [DataField(required: true)]
    public List<ProtoId<DecalPrototype>> Decals = new();
}
