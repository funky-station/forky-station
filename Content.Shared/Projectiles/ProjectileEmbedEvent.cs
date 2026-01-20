// SPDX-FileCopyrightText: 2025 Sir Warock
// SPDX-FileCopyrightText: 2024 Dakamakat
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

namespace Content.Shared.Projectiles;

/// <summary>
/// Raised directed on an entity when it embeds into something.
/// </summary>
[ByRefEvent]
public readonly record struct ProjectileEmbedEvent(EntityUid? Shooter, EntityUid? Weapon, EntityUid Embedded);
