// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Shared.EntityTable;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.PostGeneration;

/// <summary>
/// If internal areas are found will try to generate windows.
/// </summary>
public sealed partial class InternalWindowDunGen : IDunGenLayer
{
    [DataField(required: true)]
    public ProtoId<ContentTileDefinition> Tile;

    [DataField(required: true)]
    public ProtoId<EntityTablePrototype> Contents;
}
