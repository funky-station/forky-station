// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Actions.Events;

/// <summary>
/// Raised on an action entity to set its event's target to an entity, if it makes sense.
/// Does nothing for an instant action as it has no target.
/// </summary>
[ByRefEvent]
public record struct ActionSetTargetEvent(EntityUid Target, bool Handled = false);
