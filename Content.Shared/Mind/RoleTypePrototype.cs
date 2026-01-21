// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Mind;

/// <summary>
///     The core properties of Role Types
/// </summary>
[Prototype]
public sealed partial class RoleTypePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    public static readonly LocId FallbackName = "role-type-crew-aligned-name";
    public const string FallbackSymbol = "";
    public static readonly Color FallbackColor = Color.FromHex("#eeeeee");

    /// <summary>
    ///     The role's name as displayed on the UI.
    /// </summary>
    [DataField]
    public LocId Name = FallbackName;

    /// <summary>
    ///     The role's displayed color.
    /// </summary>
    [DataField]
    public Color Color = FallbackColor;

    /// <summary>
    ///     A symbol used to represent the role type.
    /// </summary>
    [DataField]
    public string Symbol = FallbackSymbol;
}
