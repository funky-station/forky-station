// SPDX-FileCopyrightText: 2025 ScarKy0
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;

namespace Content.Shared.RetractableItemAction;

/// <summary>
/// Raised when using the RetractableItem action.
/// </summary>
[ByRefEvent]
public sealed partial class OnRetractableItemActionEvent : InstantActionEvent;
