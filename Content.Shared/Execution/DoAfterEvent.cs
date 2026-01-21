// SPDX-FileCopyrightText: 2024 Scribbles0 <91828755+Scribbles0@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Execution;

[Serializable, NetSerializable]
public sealed partial class ExecutionDoAfterEvent : SimpleDoAfterEvent
{
}
