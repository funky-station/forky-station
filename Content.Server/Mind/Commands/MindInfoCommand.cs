// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2024 Errant
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2023 ShadowCommander
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using System.Text;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Mind;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Console;

namespace Content.Server.Mind.Commands
{
    [AdminCommand(AdminFlags.Admin)]
    public sealed class MindInfoCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly SharedRoleSystem _roles = default!;
        [Dependency] private readonly SharedMindSystem _minds = default!;

        public override string Command => "mindinfo";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteLine(Loc.GetString($"shell-need-exactly-one-argument"));
                return;
            }

            if (!_playerManager.TryGetSessionByUsername(args[0], out var session))
            {
                shell.WriteLine(Loc.GetString($"cmd-mindinfo-mind-not-found"));
                return;
            }

            if (!_minds.TryGetMind(session, out var mindId, out var mind))
            {
                shell.WriteLine(Loc.GetString($"cmd-mindinfo-mind-not-found"));
                return;
            }

            var builder = new StringBuilder();
            builder.AppendFormat("player: {0}, mob: {1}\nroles: ", mind.UserId, mind.OwnedEntity);

            foreach (var role in _roles.MindGetAllRoleInfo(mindId))
            {
                builder.AppendFormat("{0} ", role.Name);
            }

            shell.WriteLine(builder.ToString());
        }
    }
}
