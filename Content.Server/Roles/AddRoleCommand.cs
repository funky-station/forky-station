// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 ShadowCommander
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2021 wrexbe
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.Roles.Jobs;
using Content.Shared.Administration;
using Content.Shared.Players;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server.Roles
{
    [AdminCommand(AdminFlags.Admin)]
    public sealed class AddRoleCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly JobSystem _jobSystem = default!;

        public override string Command => "addrole";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 2)
            {
                shell.WriteLine(Loc.GetString($"shell-wrong-arguments-number-need-specific",
                    ("properAmount", 2),
                    ("currentAmount", args.Length)));
                return;
            }

            if (!_playerManager.TryGetPlayerDataByUsername(args[0], out var data))
            {
                shell.WriteLine(Loc.GetString($"cmd-addrole-mind-not-found"));
                return;
            }

            var mind = data.ContentData()?.Mind;
            if (mind == null)
            {
                shell.WriteLine(Loc.GetString($"cmd-addrole-mind-not-found"));
                return;
            }

            if (!_prototypeManager.TryIndex<JobPrototype>(args[1], out var jobPrototype))
            {
                shell.WriteLine(Loc.GetString($"cmd-addrole-role-not-found"));
                return;
            }

            if (_jobSystem.MindHasJobWithId(mind, jobPrototype.Name))
            {
                shell.WriteLine(Loc.GetString($"cmd-addrole-mind-already-has-role"));
                return;
            }

            _jobSystem.MindAddJob(mind.Value, args[1]);
        }
    }
}
