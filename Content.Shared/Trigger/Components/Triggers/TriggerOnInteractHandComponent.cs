// SPDX-FileCopyrightText: 2025 āda <ss.adasts@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Interaction;
using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Triggers;

/// <summary>
/// Trigger on <see cref="InteractHandEvent"/>, aka clicking on an entity with an empty hand.
/// User is the player with the hand doing the clicking.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnInteractHandComponent : BaseTriggerOnXComponent;
