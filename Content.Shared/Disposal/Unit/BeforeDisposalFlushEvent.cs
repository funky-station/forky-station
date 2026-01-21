// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Disposal.Unit.Events;

/// <summary>
/// Sent before the disposal unit flushes it's contents.
/// Allows adding tags for sorting and preventing the disposal unit from flushing.
/// </summary>
public sealed class BeforeDisposalFlushEvent : CancellableEntityEventArgs
{
    public readonly List<string> Tags = new();
}