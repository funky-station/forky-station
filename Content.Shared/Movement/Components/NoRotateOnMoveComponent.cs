// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components;

/// <summary>
/// This is used for entities which shouldn't have their local rotation set when moving, e.g. those using
/// <see cref="MouseRotator"/> instead
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class NoRotateOnMoveComponent : Component
{
}
