// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.Commands;
using Content.Shared.Administration;
using Content.Shared.Alert;
using Robust.Shared.Console;

namespace Content.Server.Alert.Commands
{
    [AdminCommand(AdminFlags.Debug)]
    public sealed class ClearAlert : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _e = default!;

        public string Command => "clearalert";
        public string Description => "Clears an alert for a player, defaulting to current player";
        public string Help => "clearalert <alertType> <name or userID, omit for current player>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player;
            if (player?.AttachedEntity == null)
            {
                shell.WriteLine("You don't have an entity.");
                return;
            }

            var attachedEntity = player.AttachedEntity.Value;

            if (args.Length > 1)
            {
                var target = args[1];
                if (!CommandUtils.TryGetAttachedEntityByUsernameOrId(shell, target, player, out attachedEntity)) return;
            }

            if (!_e.TryGetComponent(attachedEntity, out AlertsComponent? alertsComponent))
            {
                shell.WriteLine("user has no alerts component");
                return;
            }

            var alertType = args[0];
            var alertsSystem = _e.System<AlertsSystem>();
            if (!alertsSystem.TryGet(alertType, out var alert))
            {
                shell.WriteLine("unrecognized alertType " + alertType);
                return;
            }

            alertsSystem.ClearAlert(attachedEntity, alert.ID);
        }
    }
}
