// SPDX-FileCopyrightText: 2024 Brandon Hu
// SPDX-FileCopyrightText: 2024 Repo
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Content.Shared.GameTicking;
using Robust.Shared.Console;

namespace Content.Server.GameTicking.Commands
{
    [AnyCommand]
    sealed class ObserveCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _e = default!;
        [Dependency] private readonly IAdminManager _adminManager = default!;

        public string Command => "observe";
        public string Description => "";
        public string Help => "";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (shell.Player is not { } player)
            {
                shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
                return;
            }

            var ticker = _e.System<GameTicker>();

            if (ticker.RunLevel == GameRunLevel.PreRoundLobby)
            {
                shell.WriteError("Wait until the round starts.");
                return;
            }

            var isAdminCommand = args.Length > 0 && args[0].ToLower() == "admin";

            if (!isAdminCommand && _adminManager.IsAdmin(player))
            {
                _adminManager.DeAdmin(player);
            }

            if (ticker.PlayerGameStatuses.TryGetValue(player.UserId, out var status) &&
                status != PlayerGameStatus.JoinedGame)
            {
                ticker.JoinAsObserver(player);
            }
            else
            {
                shell.WriteError($"{player.Name} is not in the lobby.   This incident will be reported.");
            }
        }
    }
}
