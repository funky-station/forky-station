// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.GameTicking.Presets;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;

namespace Content.Server.GameTicking.Commands
{
    [AdminCommand(AdminFlags.Round)]
    public sealed class GoLobbyCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly IConfigurationManager _configManager = default!;
        [Dependency] private readonly GameTicker _gameTicker = default!;

        public override string Command => "golobby";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            GamePresetPrototype? preset = null;
            var presetName = string.Join(" ", args);

            if (args.Length > 0)
            {
                if (!_gameTicker.TryFindGamePreset(presetName, out preset))
                {
                    shell.WriteLine(Loc.GetString($"cmd-forcepreset-no-preset-found", ("preset", presetName)));
                    return;
                }
            }

            _configManager.SetCVar(CCVars.GameLobbyEnabled, true);

            _gameTicker.RestartRound();

            if (preset != null)
                _gameTicker.SetGamePreset(preset);

            shell.WriteLine(Loc.GetString(preset == null ? "cmd-golobby-success" : "cmd-golobby-success-with-preset", ("preset", presetName)));
        }
    }
}
