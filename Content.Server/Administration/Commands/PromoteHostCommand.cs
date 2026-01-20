// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Managers;
using JetBrains.Annotations;
using Robust.Server.Player;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands
{
    [UsedImplicitly]
    public sealed class PromoteHostCommand : LocalizedCommands
    {
        [Dependency] private readonly IAdminManager _adminManager = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;

        public override string Command => "promotehost";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteLine(Loc.GetString($"shell-need-exactly-one-argument"));
                return;
            }

            if (!_playerManager.TryGetSessionByUsername(args[0], out var targetPlayer))
            {
                shell.WriteLine(Loc.GetString($"shell-target-player-does-not-exist"));
                return;
            }

            _adminManager.PromoteHost(targetPlayer);
        }
    }
}
