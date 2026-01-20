// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-License-Identifier: MIT

namespace Content.Shared.Damage.Events;

/// <summary>
/// Attempting to apply stamina damage on entity.
/// </summary>
[ByRefEvent]
public record struct StaminaDamageOnHitAttemptEvent(bool Cancelled);
