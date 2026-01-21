// SPDX-FileCopyrightText: 2024-2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.EntityTable;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.PostGeneration;

/// <summary>
/// Spawns entities on either side of an entrance.
/// </summary>
public sealed partial class EntranceFlankDunGen : IDunGenLayer
{
    [DataField(required: true)]
    public ProtoId<ContentTileDefinition> Tile;

    [DataField(required: true)]
    public ProtoId<EntityTablePrototype> Contents = new();
}
