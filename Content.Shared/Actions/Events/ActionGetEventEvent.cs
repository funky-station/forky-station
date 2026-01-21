// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Actions.Events;

/// <summary>
/// Raised on an action entity to get its event.
/// </summary>
[ByRefEvent]
public record struct ActionGetEventEvent(BaseActionEvent? Event = null);
