// SPDX-FileCopyrightText: 2025 Brandon Li
// SPDX-License-Identifier: MIT

using Robust.Shared.Utility;

namespace Content.Client.Stylesheets.SheetletConfigs;

public interface ITabContainerConfig : ISheetletConfig
{
    public ResPath TabContainerPanelPath { get; }
}
