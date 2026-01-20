// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Alekshhh
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Mining;

/// <summary>
/// This is a prototype for defining ores that generate in rock
/// </summary>
[Prototype]
public sealed partial class OrePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public EntProtoId? OreEntity;

    [DataField]
    public int MinOreYield = 1;

    [DataField]
    public int MaxOreYield = 1;

    [DataField]
    public SpriteSpecifier? OreSprite;
}
