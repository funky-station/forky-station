// SPDX-FileCopyrightText: 2024 Brandon Hu
// SPDX-FileCopyrightText: 2024 Scribbles0
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2024 no
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2023 ShadowCommander
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Swept
// SPDX-FileCopyrightText: 2020 ancientpower
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2019 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 ike709
// SPDX-FileCopyrightText: 2020 Leo
// SPDX-FileCopyrightText: 2020 Sam
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-License-Identifier: MIT

using Content.Server.GameTicking;
using Content.Server.Popups;
using Content.Shared.Administration;
using Content.Shared.Chat;
using Content.Shared.Mind;
using Robust.Shared.Console;
using Robust.Shared.Enums;

namespace Content.Server.Chat.Commands
{
    [AnyCommand]
    internal sealed class SuicideCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _e = default!;

        public string Command => "suicide";

        public string Description => Loc.GetString("suicide-command-description");

        public string Help => Loc.GetString("suicide-command-help-text");

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (shell.Player is not { } player)
            {
                shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
                return;
            }

            if (player.Status != SessionStatus.InGame || player.AttachedEntity == null)
                return;

            var minds = _e.System<SharedMindSystem>();

            // This check also proves mind not-null for at the end when the mob is ghosted.
            if (!minds.TryGetMind(player, out var mindId, out var mindComp) ||
                mindComp.OwnedEntity is not { Valid: true } victim)
            {
                shell.WriteLine(Loc.GetString("suicide-command-no-mind"));
                return;
            }

            var suicideSystem = _e.System<SuicideSystem>();

            if (_e.HasComponent<AdminFrozenComponent>(victim))
            {
                var deniedMessage = Loc.GetString("suicide-command-denied");
                shell.WriteLine(deniedMessage);
                _e.System<PopupSystem>()
                    .PopupEntity(deniedMessage, victim, victim);
                return;
            }

            if (suicideSystem.Suicide(victim))
                return;

            shell.WriteLine(Loc.GetString("ghost-command-denied"));
        }
    }
}
