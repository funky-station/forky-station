// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Power.Components;

/// <summary>
/// Allows the charge of a battery to be seen by examination.
/// Requires <see cref="BatteryComponent"/>.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ExaminableBatteryComponent : Component;
