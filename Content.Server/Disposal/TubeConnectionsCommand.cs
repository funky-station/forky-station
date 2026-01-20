// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 Brandon Hu
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.Disposal.Tube;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Disposal
{
    [AdminCommand(AdminFlags.Debug)]
    public sealed class TubeConnectionsCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _entities = default!;

        public string Command => "tubeconnections";
        public string Description => Loc.GetString("tube-connections-command-description");
        public string Help => Loc.GetString("tube-connections-command-help-text", ("command", Command));

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (shell.Player is not { } player)
            {
                shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
                return;
            }

            if (player.AttachedEntity is not { } attached)
            {
                shell.WriteLine(Loc.GetString("shell-only-players-can-run-this-command"));
                return;
            }

            if (args.Length < 1)
            {
                shell.WriteLine(Help);
                return;
            }

            if (!NetEntity.TryParse(args[0], out var idNet) || !_entities.TryGetEntity(idNet, out var id))
            {
                shell.WriteLine(Loc.GetString("shell-invalid-entity-uid",("uid", args[0])));
                return;
            }

            if (!_entities.EntityExists(id))
            {
                shell.WriteLine(Loc.GetString("shell-could-not-find-entity-with-uid",("uid", id)));
                return;
            }

            if (!_entities.TryGetComponent(id, out DisposalTubeComponent? tube))
            {
                shell.WriteLine(Loc.GetString("shell-entity-with-uid-lacks-component",
                                              ("uid", id),
                                              ("componentName", nameof(DisposalTubeComponent))));
                return;
            }

            _entities.System<DisposalTubeSystem>().PopupDirections(id.Value, tube, player.AttachedEntity.Value);
        }
    }
}
