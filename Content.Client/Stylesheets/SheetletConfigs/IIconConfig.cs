// SPDX-FileCopyrightText: 2025 eoineoineoin <helloworld@eoinrul.es>
// SPDX-FileCopyrightText: 2025 Brandon Li <48413902+aspiringLich@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Utility;

namespace Content.Client.Stylesheets.SheetletConfigs;

public interface IIconConfig : ISheetletConfig
{
    public ResPath HelpIconPath { get; }
    public ResPath CrossIconPath { get; }
    public ResPath RefreshIconPath { get; }
    public ResPath InvertedTriangleIconPath { get; }
}
