// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2024 Brandon Hu
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Leo
// SPDX-License-Identifier: MIT

using Content.Server.Database;
using Content.Server.Preferences.Managers;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands
{
    [AdminCommand(AdminFlags.NameColor)]
    internal sealed class SetAdminOOC : LocalizedCommands
    {
        [Dependency] private readonly IServerDbManager _dbManager = default!;
        [Dependency] private readonly IServerPreferencesManager _preferenceManager = default!;

        public override string Command => "setadminooc";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (shell.Player == null)
            {
                shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
                return;
            }

            if (args.Length < 1)
                return;

            var colorArg = string.Join(" ", args).Trim();
            if (string.IsNullOrEmpty(colorArg))
                return;

            var color = Color.TryFromHex(colorArg);
            if (!color.HasValue)
            {
                shell.WriteError(Loc.GetString("shell-invalid-color-hex"));
                return;
            }

            var userId = shell.Player.UserId;
            // Save the DB
            _dbManager.SaveAdminOOCColorAsync(userId, color.Value);
            // Update the cached preference
            var prefs = _preferenceManager.GetPreferences(userId);
            prefs.AdminOOCColor = color.Value;
        }
    }
}
