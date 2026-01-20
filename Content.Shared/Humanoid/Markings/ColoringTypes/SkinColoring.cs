// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 csqrb
// SPDX-License-Identifier: MIT

namespace Content.Shared.Humanoid.Markings;

/// <summary>
///     Colors layer in a skin color
/// </summary>
public sealed partial class SkinColoring : LayerColoringType
{
    public override Color? GetCleanColor(Color? skin, Color? eyes, MarkingSet markingSet)
    {
        return skin;
    }
}
