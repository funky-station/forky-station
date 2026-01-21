// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components;

/// <summary>
/// Component used for tracking active two-stage triggers.
/// Used internally for performance reasons.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActiveTwoStageTriggerComponent : Component;
