// SPDX-FileCopyrightText: 2023-2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Emisse <99158783+Emisse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Maps;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.PostGeneration;

/// <summary>
/// Connects room entrances via corridor segments.
/// </summary>
public sealed partial class CorridorDunGen : IDunGenLayer
{
    /// <summary>
    /// How far we're allowed to generate a corridor before calling it.
    /// </summary>
    /// <remarks>
    /// Given the heavy weightings this needs to be fairly large for larger dungeons.
    /// </remarks>
    [DataField]
    public int PathLimit = 2048;

    /// <summary>
    /// How wide to make the corridor.
    /// </summary>
    [DataField]
    public float Width = 3f;

    [DataField(required: true)]
    public ProtoId<ContentTileDefinition> Tile;
}
