// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

namespace Content.Shared.Weapons.Melee.Events;

/// <summary>
/// Raised directed on a weapon when attempt a melee attack.
/// </summary>
[ByRefEvent]
public record struct AttemptMeleeEvent(EntityUid User, bool Cancelled = false, string? Message = null);
