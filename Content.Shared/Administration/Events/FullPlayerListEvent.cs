// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Events
{
    [Serializable, NetSerializable]
    public sealed class FullPlayerListEvent : EntityEventArgs
    {
        public List<PlayerInfo> PlayersInfo = new();
    }
}
