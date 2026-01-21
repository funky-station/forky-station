// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Centronias <me@centronias.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Emoting;

public sealed class EmoteAttemptEvent(EntityUid uid) : CancellableEntityEventArgs
{
    public EntityUid Uid { get; } = uid;
}
