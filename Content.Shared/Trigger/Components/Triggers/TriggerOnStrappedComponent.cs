// SPDX-FileCopyrightText: 2025 Studio Fae-Wilds <studio.faewilds@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Triggers;

/// <summary>
/// Triggers when something is strapped to the entity.
/// This is intended to be used on objects like chairs or beds.
/// The user is the entity strapped to the component owner.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnStrappedComponent : BaseTriggerOnXComponent;
