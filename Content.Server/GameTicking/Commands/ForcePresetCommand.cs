// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Radosvik
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Server.Administration;
using Content.Server.GameTicking.Presets;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Commands
{
    [AdminCommand(AdminFlags.Round)]
    public sealed class ForcePresetCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly GameTicker _ticker = default!;

        public override string Command => "forcepreset";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (_ticker.RunLevel != GameRunLevel.PreRoundLobby)
            {
                shell.WriteLine(Loc.GetString($"cmd-forcepreset-preround-lobby-only"));
                return;
            }

            if (args.Length != 1)
            {
                shell.WriteLine(Loc.GetString($"shell-need-exactly-one-argument"));
                return;
            }

            var name = args[0];
            if (!_ticker.TryFindGamePreset(name, out var type))
            {
                shell.WriteLine(Loc.GetString($"cmd-forcepreset-no-preset-found", ("preset", name)));
                return;
            }

            _ticker.SetGamePreset(type, true);
            shell.WriteLine(Loc.GetString($"cmd-forcepreset-success", ("preset", name)));
            _ticker.UpdateInfoText();
        }

        public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            if (args.Length == 1)
            {
                var options = _prototypeManager
                    .EnumeratePrototypes<GamePresetPrototype>()
                    .OrderBy(p => p.ID)
                    .Select(p => p.ID);

                return CompletionResult.FromHintOptions(options, Loc.GetString($"cmd-forcepreset-hint"));
            }

            return CompletionResult.Empty;
        }
    }
}
