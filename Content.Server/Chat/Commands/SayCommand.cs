// SPDX-FileCopyrightText: 2020-2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021-2023 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2021 Moony <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2022-2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 Michael Phillips <1194692+MeltedPixel@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 HerCoyote23 <131214189+HerCoyote23@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 20kdc <asdd2808@gmail.com>
// SPDX-FileCopyrightText: 2024 Brandon Hu <103440971+Brandon-Huu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using Content.Shared.Chat;
using Robust.Shared.Console;
using Robust.Shared.Enums;

namespace Content.Server.Chat.Commands
{
    [AnyCommand]
    internal sealed class SayCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly ChatSystem _chatSystem = default!;
        public override string Command => "say";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (shell.Player is not { } player)
            {
                shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
                return;
            }

            if (player.Status != SessionStatus.InGame)
                return;

            if (player.AttachedEntity is not {} playerEntity)
            {
                shell.WriteError(Loc.GetString($"shell-must-be-attached-to-entity"));
                return;
            }

            if (args.Length < 1)
                return;

            var message = string.Join(" ", args).Trim();
            if (string.IsNullOrEmpty(message))
                return;

            _chatSystem.TrySendInGameICMessage(playerEntity, message, InGameICChatType.Speak, ChatTransmitRange.Normal, false, shell, player);
        }
    }
}
