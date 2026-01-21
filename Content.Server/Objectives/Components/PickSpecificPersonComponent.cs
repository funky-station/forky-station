// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Server.Objectives.Components;

/// <summary>
/// Sets this objective's target to the one given in <see cref="TargetOverrideComponent"/>, if the entity has it.
/// This component needs to be added to objective entity itself.
/// </summary>
[RegisterComponent]
public sealed partial class PickSpecificPersonComponent : Component;
