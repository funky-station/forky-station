// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Containers;

/// <summary>
/// Sent before the insertion is made.
/// Allows preventing the insertion if any system on the entity should need to.
/// </summary>
[ByRefEvent]
public record struct BeforeThrowInsertEvent(EntityUid ThrownEntity, bool Cancelled = false);
