// SPDX-FileCopyrightText: 2021, 2025 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021, 2023 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Brandon Hu <103440971+Brandon-Huu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.EntityList;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server.EntityList
{
    [AdminCommand(AdminFlags.Spawn)]
    public sealed class SpawnEntityListCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public override string Command => "spawnentitylist";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteError(Loc.GetString($"shell-need-exactly-one-argument"));
                return;
            }

            if (shell.Player is not { } player)
            {
                shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
                return;
            }

            if (player.AttachedEntity is not {} attached)
            {
                shell.WriteError(Loc.GetString("shell-only-players-can-run-this-command"));
                return;
            }

            if (!_prototypeManager.TryIndex(args[0], out EntityListPrototype? prototype))
            {
                shell.WriteError(Loc.GetString($"cmd-spawnentitylist-failed",
                    ("prototype", nameof(EntityListPrototype)),
                    ("id", args[0])));
                return;
            }

            var i = 0;

            foreach (var entity in prototype.GetEntities(_prototypeManager))
            {
                EntityManager.SpawnEntity(entity.ID, EntityManager.GetComponent<TransformComponent>(attached).Coordinates);
                i++;
            }

            shell.WriteLine(Loc.GetString($"cmd-spawnentitylist-success", ("count", i)));
        }
    }
}
