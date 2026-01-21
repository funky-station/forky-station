// SPDX-FileCopyrightText: 2025 Brandon Li <48413902+aspiringLich@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using JetBrains.Annotations;

namespace Content.Client.Stylesheets;

/// <summary>
///     Attribute used to mark a sheetlet class. Stylesheets can use this attribute to locate and load sheetlets.
/// </summary>
[PublicAPI]
[MeansImplicitUse]
public sealed class CommonSheetletAttribute : Attribute
{

}
