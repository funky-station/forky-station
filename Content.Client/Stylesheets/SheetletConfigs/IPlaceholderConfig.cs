// SPDX-FileCopyrightText: 2025 Brandon Li <48413902+aspiringLich@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Utility;

namespace Content.Client.Stylesheets.SheetletConfigs;

public interface IPlaceholderConfig : ISheetletConfig
{
    public ResPath PlaceholderPath { get; }
}

