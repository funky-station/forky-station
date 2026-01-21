// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Effects;

/// <summary>
/// Rejuvenates the entity when triggered, which fully heals them, removing all damage, debuffs or other negative status effects.
/// If TargetUser is true then the user will be rejuvenated instead.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RejuvenateOnTriggerComponent : BaseXOnTriggerComponent;
