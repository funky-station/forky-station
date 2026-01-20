// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

namespace Content.Shared.Interaction.Events
{
    public sealed class UseAttemptEvent(EntityUid uid, EntityUid used) : CancellableEntityEventArgs
    {
        public EntityUid Uid { get; } = uid;

        public EntityUid Used = used;
    }
}
