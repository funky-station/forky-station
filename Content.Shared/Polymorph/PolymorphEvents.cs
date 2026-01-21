// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Polymorph;

/// <summary>
/// Raised locally on an entity when it polymorphs into another entity
/// </summary>
/// <param name="OldEntity">EntityUid of the entity before the polymorph</param>
/// <param name="NewEntity">EntityUid of the entity after the polymorph</param>
/// <param name="IsRevert">Whether this polymorph event was a revert back to the original entity</param>
[ByRefEvent]
public record struct PolymorphedEvent(EntityUid OldEntity, EntityUid NewEntity, bool IsRevert);
