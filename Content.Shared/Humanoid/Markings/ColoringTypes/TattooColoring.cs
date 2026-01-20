// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 csqrb
// SPDX-License-Identifier: MIT

namespace Content.Shared.Humanoid.Markings;

/// <summary>
///     Colors layer in skin color but much darker.
/// </summary>
public sealed partial class TattooColoring : LayerColoringType
{
    public override Color? GetCleanColor(Color? skin, Color? eyes, MarkingSet markingSet)
    {
        if (skin == null)
        {
            return null;
        }

        var newColor = Color.ToHsv(skin.Value);
        newColor.Z = .40f;

        return Color.FromHsv(newColor);
    }
}
