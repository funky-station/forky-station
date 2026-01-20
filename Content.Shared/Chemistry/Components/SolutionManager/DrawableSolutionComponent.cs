// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Emisse
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-License-Identifier: MIT

namespace Content.Shared.Chemistry.Components.SolutionManager;

/// <summary>
///     Denotes the solution that can removed  be with syringes.
/// </summary>
[RegisterComponent]
public sealed partial class DrawableSolutionComponent : Component
{
    /// <summary>
    /// Solution name that can be removed with syringes.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string Solution = "default";
}
