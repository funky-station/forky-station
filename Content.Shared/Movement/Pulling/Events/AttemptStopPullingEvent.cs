// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2025 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Pulling.Events;

/// <summary>
/// Raised when a request is made to stop pulling an entity.
/// </summary>

[ByRefEvent]
public record struct AttemptStopPullingEvent(EntityUid? User = null)
{
    public readonly EntityUid? User = User;
    public bool Cancelled;
}
