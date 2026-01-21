// SPDX-FileCopyrightText: 2024-2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.PostGeneration;

/// <summary>
/// Runs cables throughout the dungeon.
/// </summary>
public sealed partial class AutoCablingDunGen : IDunGenLayer
{
    [DataField(required: true)]
    public EntProtoId Entity;
}
