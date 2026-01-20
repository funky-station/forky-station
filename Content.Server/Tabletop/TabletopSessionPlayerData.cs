// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

namespace Content.Server.Tabletop
{
    /// <summary>
    ///     A class that stores per-player data for tabletops.
    /// </summary>
    public sealed class TabletopSessionPlayerData
    {
        public EntityUid Camera { get; set; }
    }
}
