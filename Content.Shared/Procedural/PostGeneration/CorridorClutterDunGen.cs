// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 Emisse
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Shared.EntityTable;
using Content.Shared.Storage;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.PostGeneration;

/// <summary>
/// Adds entities randomly to the corridors.
/// </summary>
public sealed partial class CorridorClutterDunGen : IDunGenLayer
{
    [DataField]
    public float Chance = 0.05f;

    /// <summary>
    /// The default starting bulbs
    /// </summary>
    [DataField(required: true)]
    public ProtoId<EntityTablePrototype> Contents;
}
