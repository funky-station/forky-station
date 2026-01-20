// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-License-Identifier: MIT

namespace Content.Shared.Throwing
{
    public sealed class ThrowAttemptEvent : CancellableEntityEventArgs
    {
        public ThrowAttemptEvent(EntityUid uid, EntityUid itemUid)
        {
            Uid = uid;
            ItemUid = itemUid;
        }

        public EntityUid Uid { get; }

        public EntityUid ItemUid { get; }
    }

    /// <summary>
    ///     Raised on the item entity that is thrown.
    /// </summary>
    /// <param name="User">The user that threw this entity.</param>
    /// <param name="Cancelled">Whether or not the throw should be cancelled.</param>
    [ByRefEvent]
    public record struct ThrowItemAttemptEvent(EntityUid User, bool Cancelled = false);

    /// <summary>
    /// Raised when we try to pushback an entity from throwing
    /// </summary>
    public sealed class ThrowPushbackAttemptEvent : CancellableEntityEventArgs {}

    /// <summary>
    /// Raised on an entity that is being pushed from a thrown entity
    /// </summary>
    [ByRefEvent]
    public record struct ThrowerImpulseEvent()
    {
        public bool Push;
    };
}
