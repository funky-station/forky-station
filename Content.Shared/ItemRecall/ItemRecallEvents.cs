// SPDX-FileCopyrightText: 2025 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;

namespace Content.Shared.ItemRecall;

/// <summary>
/// Raised when using the ItemRecall action.
/// </summary>
[ByRefEvent]
public sealed partial class OnItemRecallActionEvent : InstantActionEvent;
