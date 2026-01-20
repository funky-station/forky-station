// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Jezithyr
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

namespace Content.Shared.Pulling.Events
{
    /// <summary>
    ///     Directed event raised on the pulled to see if it can be pulled.
    /// </summary>
    public sealed class BeingPulledAttemptEvent : CancellableEntityEventArgs
    {
        public BeingPulledAttemptEvent(EntityUid puller, EntityUid pulled)
        {
            Puller = puller;
            Pulled = pulled;
        }

        public EntityUid Puller { get; }
        public EntityUid Pulled { get; }
    }
}
