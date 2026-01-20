// SPDX-FileCopyrightText: 2023 Jezithyr
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 moonheart08
// SPDX-License-Identifier: MIT

using Content.Server.Body.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands
{
    [AdminCommand(AdminFlags.Admin)]
    public sealed class RemoveBodyPartCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _entManager = default!;

        public string Command => "rmbodypart";
        public string Description => "Removes a given entity from it's containing body, if any.";
        public string Help => "Usage: rmbodypart <uid>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
                return;
            }

            if (!NetEntity.TryParse(args[0], out var entityUidNet) || !_entManager.TryGetEntity(entityUidNet, out var entityUid))
            {
                shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
                return;
            }

            // TODO: THIS IS JUST A MECHANISM COPYPASTE
            var xformSystem = _entManager.System<SharedTransformSystem>();
            xformSystem.AttachToGridOrMap(entityUid.Value);
            shell.WriteLine($"Removed body part {_entManager.ToPrettyString(entityUid.Value)}");
        }
    }
}
