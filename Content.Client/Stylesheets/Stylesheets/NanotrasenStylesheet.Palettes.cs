// SPDX-FileCopyrightText: 2025 Brandon Li <48413902+aspiringLich@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Client.Stylesheets.Palette;

namespace Content.Client.Stylesheets.Stylesheets;

public sealed partial class NanotrasenStylesheet
{
    public override ColorPalette PrimaryPalette => Palettes.Navy;
    public override ColorPalette SecondaryPalette => Palettes.Slate;
    public override ColorPalette PositivePalette => Palettes.Green;
    public override ColorPalette NegativePalette => Palettes.Red;
    public override ColorPalette HighlightPalette => Palettes.Gold;
}
