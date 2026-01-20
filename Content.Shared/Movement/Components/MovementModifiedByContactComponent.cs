// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Components;

/// <summary>
/// Exists just to listen to a single event. What a life.
/// </summary>
[NetworkedComponent, RegisterComponent] // must be networked to properly predict adding & removal
public sealed partial class SpeedModifiedByContactComponent : Component
{
}

[NetworkedComponent, RegisterComponent] // ditto but for friction
public sealed partial class FrictionModifiedByContactComponent : Component
{
}
