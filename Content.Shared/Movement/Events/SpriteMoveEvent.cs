// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Movement.Events;

/// <summary>
/// Raised on an entity whenever it should change movement sprite
/// </summary>
[ByRefEvent]
public readonly struct SpriteMoveEvent
{
    public readonly bool IsMoving = false;

    public SpriteMoveEvent(bool isMoving)
    {
        IsMoving = isMoving;
    }
}
