// SPDX-FileCopyrightText: 2024-2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.EntityTable;
using Content.Shared.Storage;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.PostGeneration;

/// <summary>
/// Spawns entities inside corners.
/// </summary>
public sealed partial class CornerClutterDunGen : IDunGenLayer
{
    [DataField]
    public float Chance = 0.50f;

    [DataField(required:true)]
    public ProtoId<EntityTablePrototype> Contents = new();
}
