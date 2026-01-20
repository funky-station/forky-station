// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Atmos.Commands
{
    [AdminCommand(AdminFlags.Debug)]
    public sealed class ListGasesCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _e = default!;

        public string Command => "listgases";
        public string Description => "Prints a list of gases and their indices.";
        public string Help => "listgases";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var atmosSystem = _e.System<AtmosphereSystem>();

            foreach (var gasPrototype in atmosSystem.Gases)
            {
                var gasName = Loc.GetString(gasPrototype.Name);
                shell.WriteLine($"{gasName} ID: {gasPrototype.ID}");
            }
        }
    }

}
