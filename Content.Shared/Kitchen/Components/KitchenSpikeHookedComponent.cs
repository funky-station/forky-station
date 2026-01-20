// SPDX-FileCopyrightText: 2025 Winkarst-cpu
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Kitchen.Components;

/// <summary>
/// Used to mark entities that are currently hooked on the spike.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedKitchenSpikeSystem))]
public sealed partial class KitchenSpikeHookedComponent : Component;
