// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-License-Identifier: MIT

namespace Content.Server.Lathe.Components;

/// <summary>
/// For EntityQuery to keep track of which lathes are producing
/// </summary>
[RegisterComponent]
public sealed partial class LatheProducingComponent : Component
{
    /// <summary>
    /// The time at which production began
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan StartTime;

    /// <summary>
    /// How long it takes to produce the recipe.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan ProductionLength;
}

