// SPDX-FileCopyrightText: 2025 Studio Fae-Wilds <studio.faewilds@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Triggers;

/// <summary>
/// Triggers when the entity exits a floating or thrown state and lands on a surface.
/// The user is the thrower.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnLandComponent : BaseTriggerOnXComponent;
