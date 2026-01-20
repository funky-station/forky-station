// SPDX-FileCopyrightText: 2025 Brandon Li
// SPDX-License-Identifier: MIT

using Robust.Shared.Utility;

namespace Content.Client.Stylesheets.SheetletConfigs;

public interface INanoHeadingConfig : ISheetletConfig
{
    public ResPath NanoHeadingPath { get;  }
}

