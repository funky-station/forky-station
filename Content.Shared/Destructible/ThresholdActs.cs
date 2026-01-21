// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Destructible;

[Flags]
[Serializable, NetSerializable]
public enum ThresholdActs : byte
{
    None = 0,
    Breakage = 1 << 0,
    Destruction = 1 << 1,
}
