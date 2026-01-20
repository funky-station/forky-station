// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

using Robust.Shared.Network;

namespace Content.Server.Database
{
    public sealed class ServerUnbanDef
    {
        public int BanId { get; }

        public NetUserId? UnbanningAdmin { get; }

        public DateTimeOffset UnbanTime { get; }

        public ServerUnbanDef(int banId, NetUserId? unbanningAdmin, DateTimeOffset unbanTime)
        {
            BanId = banId;
            UnbanningAdmin = unbanningAdmin;
            UnbanTime = unbanTime;
        }
    }
}
