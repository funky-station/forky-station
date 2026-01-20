// SPDX-FileCopyrightText: 2025 Errant
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Jezithyr
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
