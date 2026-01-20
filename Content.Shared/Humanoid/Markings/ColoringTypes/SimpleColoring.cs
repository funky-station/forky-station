// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 csqrb
// SPDX-License-Identifier: MIT

namespace Content.Shared.Humanoid.Markings;

/// <summary>
///     Colors layer in a specified color
/// </summary>
public sealed partial class SimpleColoring : LayerColoringType
{
    [DataField("color", required: true)]
    public Color Color = Color.White;

    public override Color? GetCleanColor(Color? skin, Color? eyes, MarkingSet markingSet)
    {
        return Color;
    }
}
