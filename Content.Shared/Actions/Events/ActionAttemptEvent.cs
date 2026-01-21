// SPDX-FileCopyrightText: 2024-2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Actions.Events;

/// <summary>
/// Raised before an action is used and can be cancelled to prevent it.
/// Allowed to have side effects like modifying the action components.
/// </summary>
[ByRefEvent]
public record struct ActionAttemptEvent(EntityUid User, bool Cancelled = false);
