// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-License-Identifier: MIT

namespace Content.Shared.Procedural.PostGeneration;

/// <summary>
/// Applies decal skirting to corridors.
/// </summary>
public sealed partial class CorridorDecalSkirtingDunGen : IDunGenLayer
{
    /// <summary>
    /// Decal where 1 edge is found.
    /// </summary>
    [DataField]
    public Dictionary<DirectionFlag, string> CardinalDecals = new();

    /// <summary>
    /// Decal where 1 corner edge is found.
    /// </summary>
    [DataField]
    public Dictionary<Direction, string> PocketDecals = new();

    /// <summary>
    /// Decal where 2 or 3 edges are found.
    /// </summary>
    [DataField]
    public Dictionary<DirectionFlag, string> CornerDecals = new();

    /// <summary>
    /// Optional color to apply to the decals.
    /// </summary>
    [DataField]
    public Color? Color;
}
