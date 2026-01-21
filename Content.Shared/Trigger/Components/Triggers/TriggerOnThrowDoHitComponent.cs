// SPDX-FileCopyrightText: 2025 āda <ss.adasts@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Triggers;

/// <summary>
/// Trigger for when this entity is thrown and then hits a second entity.
/// User is the entity hit.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnThrowDoHitComponent : BaseTriggerOnXComponent;
