// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-License-Identifier: MIT

namespace Content.Server.Ninja.Events;

/// <summary>
/// Raised on the ninja and suit when the suit has its powercell changed.
/// </summary>
[ByRefEvent]
public record struct NinjaBatteryChangedEvent(EntityUid Battery, EntityUid BatteryHolder);
