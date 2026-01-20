// SPDX-FileCopyrightText: 2024 brainfood1183
// SPDX-License-Identifier: MIT

namespace Content.Shared.Actions.Events;

public sealed partial class FireStarterActionEvent : InstantActionEvent
{
    /// <summary>
    /// Increases the number of fire stacks when a flammable object is ignited.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float Severity = 0.3f;
}
