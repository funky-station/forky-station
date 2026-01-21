// SPDX-FileCopyrightText: 2025 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Revolutionary;
using Robust.Shared.GameStates;

namespace Content.Shared.Mindshield.Components;

/// <summary>
/// Component given to an entity to mark it is a mindshield implant.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedRevolutionarySystem))]
public sealed partial class MindShieldImplantComponent : Component;
