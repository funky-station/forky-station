// SPDX-FileCopyrightText: 2024 themias
// SPDX-License-Identifier: MIT

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Burial;

[Serializable, NetSerializable]
public sealed partial class GraveDiggingDoAfterEvent : SimpleDoAfterEvent
{
}
