// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 Samuka-C <47865393+Samuka-C@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Radio.EntitySystems;
using Robust.Shared.GameStates;

namespace Content.Shared.Radio.Components;

/// <summary>
/// Prevents all non whitelisted radios from sending messages
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedJammerSystem))]
public sealed partial class ActiveRadioJammerComponent : Component
{
}
