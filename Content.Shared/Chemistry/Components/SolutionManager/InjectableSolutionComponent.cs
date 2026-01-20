// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Emisse
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-License-Identifier: MIT

namespace Content.Shared.Chemistry.Components.SolutionManager;

/// <summary>
///     Denotes a solution which can be added with syringes.
/// </summary>
[RegisterComponent]
public sealed partial class InjectableSolutionComponent : Component
{

    /// <summary>
    /// Solution name which can be added with syringes.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string Solution = "default";
}
