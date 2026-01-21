// SPDX-FileCopyrightText: 2023, 2025 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023, 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
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
