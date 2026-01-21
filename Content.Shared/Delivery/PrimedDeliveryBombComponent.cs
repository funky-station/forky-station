// SPDX-FileCopyrightText: 2025 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Delivery;

/// <summary>
/// Component given to deliveries.
/// Indicates this bomb delivery is primed.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(DeliveryModifierSystem))]
public sealed partial class PrimedDeliveryBombComponent : Component;
