// SPDX-FileCopyrightText: 2025 Brandon Li <48413902+aspiringLich@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Utility;

namespace Content.Client.Stylesheets.SheetletConfigs;

public interface IWindowConfig : ISheetletConfig
{
    public ResPath WindowHeaderTexturePath { get; }
    public ResPath WindowHeaderAlertTexturePath { get; }
    public ResPath WindowBackgroundPath { get; }
    public ResPath WindowBackgroundBorderedPath { get; }
    public ResPath TransparentWindowBackgroundBorderedPath { get; }
}
