// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

namespace Content.Shared.Interaction.Events;

/// <summary>
/// Raised directed on the target entity when being attacked.
/// </summary>
[ByRefEvent]
public record struct GettingAttackedAttemptEvent(EntityUid Attacker, EntityUid? Weapon, bool Disarm, bool Cancelled = false);
