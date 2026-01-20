// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Shared.Maps;
using Robust.Shared.Prototypes;

namespace Content.Shared.Parallax.Biomes.Layers;

/// <summary>
/// Handles actual objects such as decals and entities.
/// </summary>
public partial interface IBiomeWorldLayer : IBiomeLayer
{
    /// <summary>
    /// What tiles we're allowed to spawn on, real or biome.
    /// </summary>
    List<ProtoId<ContentTileDefinition>> AllowedTiles { get; }
}
