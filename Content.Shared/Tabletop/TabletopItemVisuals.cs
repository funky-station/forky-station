// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop
{
    [Serializable, NetSerializable]
    public enum TabletopItemVisuals : byte
    {
        Scale,
        DrawDepth
    }
}
