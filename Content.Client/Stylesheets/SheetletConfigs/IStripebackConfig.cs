// SPDX-FileCopyrightText: 2025 Brandon Li
// SPDX-License-Identifier: MIT

using Robust.Shared.Utility;

namespace Content.Client.Stylesheets.SheetletConfigs;

public interface IStripebackConfig : ISheetletConfig
{
    public ResPath StripebackPath { get; }
}
