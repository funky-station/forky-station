// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Triggers;

/// <summary>
/// Triggers when activating an action granted by an implant.
/// The user is the player activating it.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnActivateImplantComponent : BaseTriggerOnXComponent;
