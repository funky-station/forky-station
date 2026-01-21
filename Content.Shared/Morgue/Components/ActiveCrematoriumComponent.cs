// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Morgue.Components;

/// <summary>
/// Used to track actively cooking crematoriums.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActiveCrematoriumComponent : Component;
