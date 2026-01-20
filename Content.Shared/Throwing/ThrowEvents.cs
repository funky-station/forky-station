// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

namespace Content.Shared.Throwing;

/// <summary>
/// Raised on an entity after it has thrown something.
/// </summary>
[ByRefEvent]
public readonly record struct ThrowEvent(EntityUid? User, EntityUid Thrown);

/// <summary>
/// Raised on an entity after it has been thrown.
/// </summary>
[ByRefEvent]
public readonly record struct ThrownEvent(EntityUid? User, EntityUid Thrown);

/// <summary>
/// Raised directed on the target entity being hit by the thrown entity.
/// </summary>
[ByRefEvent]
public readonly record struct ThrowHitByEvent(EntityUid Thrown, EntityUid Target, ThrownItemComponent Component);

/// <summary>
/// Raised directed on the thrown entity that hits another.
/// </summary>
[ByRefEvent]
public readonly record struct ThrowDoHitEvent(EntityUid Thrown, EntityUid Target, ThrownItemComponent Component);
