// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Throwing;

/// <summary>
/// Raised on someone when they try to catch an item.
/// </summary>
[ByRefEvent]
public record struct CatchAttemptEvent(EntityUid Item, float CatchChance, bool Cancelled = false);
