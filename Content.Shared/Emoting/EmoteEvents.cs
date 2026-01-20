// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 Centronias
// SPDX-License-Identifier: MIT

namespace Content.Shared.Emoting;

public sealed class EmoteAttemptEvent(EntityUid uid) : CancellableEntityEventArgs
{
    public EntityUid Uid { get; } = uid;
}
