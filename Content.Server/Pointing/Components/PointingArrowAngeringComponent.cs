// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-License-Identifier: MIT

namespace Content.Server.Pointing.Components;

/// <summary>
/// Causes pointing arrows to go mode and murder this entity.
/// </summary>
[RegisterComponent]
public sealed partial class PointingArrowAngeringComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("remainingAnger")]
    public int RemainingAnger = 5;
}
