// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2024 Brandon Hu
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Michael Phillips
// SPDX-FileCopyrightText: 2022 Morbo
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using Content.Shared.Chat;
using Robust.Shared.Console;
using Robust.Shared.Enums;

namespace Content.Server.Chat.Commands
{
    [AnyCommand]
    internal sealed class LOOCCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _e = default!;

        public string Command => "looc";
        public string Description => "Send Local Out Of Character chat messages.";
        public string Help => "looc <text>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (shell.Player is not { } player)
            {
                shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
                return;
            }

            if (player.AttachedEntity is not { Valid: true } entity)
                return;

            if (player.Status != SessionStatus.InGame)
                return;

            if (args.Length < 1)
                return;

            var message = string.Join(" ", args).Trim();
            if (string.IsNullOrEmpty(message))
                return;

            _e.System<ChatSystem>().TrySendInGameOOCMessage(entity, message, InGameOOCChatType.Looc, false, shell, player);
        }
    }
}
