// SPDX-FileCopyrightText: 2023 Arendian <137322659+Arendian@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Weapons.Ranged.Events;

/// <summary>
/// Raised directed on the gun when trying to fire it while it's out of ammo
/// </summary>
[ByRefEvent]
public record struct OnEmptyGunShotEvent(EntityUid User);
