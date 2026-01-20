// SPDX-FileCopyrightText: 2025 ElectroJr
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 moonheart08
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-License-Identifier: MIT

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Commands.Generic;

namespace Content.Server.Commands
{
    /// <summary>
    /// Utilities for writing commands
    /// </summary>
    public static class CommandUtils
    {
        /// <summary>
        /// Gets the player session for the player with the indicated id,
        /// sending a failure to the performer if unable to.
        /// </summary>
        public static bool TryGetSessionByUsernameOrId(IConsoleShell shell,
            string usernameOrId, ICommonSession performer, [NotNullWhen(true)] out ICommonSession? session)
        {
            var plyMgr = IoCManager.Resolve<IPlayerManager>();
            if (plyMgr.TryGetSessionByUsername(usernameOrId, out session)) return true;
            if (Guid.TryParse(usernameOrId, out var targetGuid))
            {
                if (plyMgr.TryGetSessionById(new NetUserId(targetGuid), out session)) return true;
                shell.WriteLine("Unable to find user with that name/id.");
                return false;
            }

            shell.WriteLine("Unable to find user with that name/id.");
            return false;
        }

        /// <summary>
        /// Gets the attached entity for the player session with the indicated id,
        /// sending a failure to the performer if unable to.
        /// </summary>
        public static bool TryGetAttachedEntityByUsernameOrId(IConsoleShell shell,
            string usernameOrId, ICommonSession performer, out EntityUid attachedEntity)
        {
            attachedEntity = default;
            if (!TryGetSessionByUsernameOrId(shell, usernameOrId, performer, out var session)) return false;
            if (session.AttachedEntity == null)
            {
                shell.WriteLine("User has no attached entity.");
                return false;
            }

            attachedEntity = session.AttachedEntity.Value;
            return true;
        }
    }
}
