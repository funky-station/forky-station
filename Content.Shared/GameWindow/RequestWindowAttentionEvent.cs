// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.GameWindow
{
    [Serializable, NetSerializable]
    public sealed class RequestWindowAttentionEvent : EntityEventArgs
    {
    }
}
