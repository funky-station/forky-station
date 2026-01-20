// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2024 lzk
// SPDX-FileCopyrightText: 2024 Schrödinger
// SPDX-FileCopyrightText: 2024 Winkarst
// SPDX-FileCopyrightText: 2024 no
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 20kdc
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Manel Navola
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 NuclearWinter
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 ancientpower
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-License-Identifier: MIT

using Content.Server.Popups;
using Content.Shared.Administration;
using Content.Shared.GameTicking;
using Content.Shared.Mind;
using Robust.Shared.Console;
using Content.Server.GameTicking;

namespace Content.Server.Ghost
{
    [AnyCommand]
    public sealed class GhostCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _entities = default!;

        public string Command => "ghost";
        public string Description => Loc.GetString("ghost-command-description");
        public string Help => Loc.GetString("ghost-command-help-text");

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player;
            if (player == null)
            {
                shell.WriteLine(Loc.GetString("ghost-command-no-session"));
                return;
            }

            var gameTicker = _entities.System<GameTicker>();
            if (!gameTicker.PlayerGameStatuses.TryGetValue(player.UserId, out var playerStatus) ||
                playerStatus is not PlayerGameStatus.JoinedGame)
            {
                shell.WriteLine(Loc.GetString("ghost-command-error-lobby"));
                return;
            }

            if (player.AttachedEntity is { Valid: true } frozen &&
                _entities.HasComponent<AdminFrozenComponent>(frozen))
            {
                var deniedMessage = Loc.GetString("ghost-command-denied");
                shell.WriteLine(deniedMessage);
                _entities.System<PopupSystem>()
                    .PopupEntity(deniedMessage, frozen, frozen);
                return;
            }

            var minds = _entities.System<SharedMindSystem>();
            if (!minds.TryGetMind(player, out var mindId, out var mind))
            {
                mindId = minds.CreateMind(player.UserId);
                mind = _entities.GetComponent<MindComponent>(mindId);
            }

            if (!_entities.System<GhostSystem>().OnGhostAttempt(mindId, true, true, mind: mind))
            {
                shell.WriteLine(Loc.GetString("ghost-command-denied"));
            }
        }
    }
}
