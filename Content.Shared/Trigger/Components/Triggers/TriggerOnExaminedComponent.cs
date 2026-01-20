// SPDX-FileCopyrightText: 2025 Studio Fae-Wilds
// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Triggers;

/// <summary>
/// Triggers when the entity is being examined.
/// The user is the player doing the examination.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TriggerOnExaminedComponent : BaseTriggerOnXComponent;
