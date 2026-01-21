// SPDX-FileCopyrightText: 2025 Centronias <me@centronias.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.ParcelWrap.Components;

/// <summary>
/// This enum is used to change the sprite used by WrappedParcels based on the parcel's size.
/// </summary>
[Serializable, NetSerializable]
public enum WrappedParcelVisuals : byte
{
    Size,
    Layer,
}
