// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 poklj <compgeek223@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Changeling;

/// <summary>
/// Action event for opening the changeling transformation radial menu.
/// </summary>
public sealed partial class ChangelingTransformActionEvent : InstantActionEvent;

/// <summary>
/// DoAfterevent used to transform a changeling into one of their stored identities.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class ChangelingTransformDoAfterEvent : SimpleDoAfterEvent;
