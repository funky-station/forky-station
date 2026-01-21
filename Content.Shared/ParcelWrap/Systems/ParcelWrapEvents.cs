// SPDX-FileCopyrightText: 2025 Centronias <me@centronias.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.ParcelWrap.Systems;

[Serializable, NetSerializable]
public sealed partial class ParcelWrapItemDoAfterEvent : SimpleDoAfterEvent;

[Serializable, NetSerializable]
public sealed partial class UnwrapWrappedParcelDoAfterEvent : SimpleDoAfterEvent;
