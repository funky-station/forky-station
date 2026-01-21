// SPDX-FileCopyrightText: 2021-2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2021 Moony <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Mervill <mervills.email@gmail.com>
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 ArkiveDev <95712736+ArkiveDev@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Server.Administration;
using Content.Server.Maps;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Maps;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Commands
{
    [AdminCommand(AdminFlags.Round)]
    public sealed class ForceMapCommand : LocalizedCommands
    {
        [Dependency] private readonly IConfigurationManager _configurationManager = default!;
        [Dependency] private readonly IGameMapManager _gameMapManager = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public override string Command => "forcemap";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteLine(Loc.GetString(Loc.GetString($"shell-need-exactly-one-argument")));
                return;
            }

            var name = args[0];

            // An empty string clears the forced map
            if (!string.IsNullOrEmpty(name) && !_gameMapManager.CheckMapExists(name))
            {
                shell.WriteLine(Loc.GetString("cmd-forcemap-map-not-found", ("map", name)));
                return;
            }

            _configurationManager.SetCVar(CCVars.GameMap, name);

            if (string.IsNullOrEmpty(name))
                shell.WriteLine(Loc.GetString("cmd-forcemap-cleared"));
            else
                shell.WriteLine(Loc.GetString("cmd-forcemap-success", ("map", name)));
        }

        public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            if (args.Length == 1)
            {
                var options = _prototypeManager
                    .EnumeratePrototypes<GameMapPrototype>()
                    .Select(p => new CompletionOption(p.ID, p.MapName))
                    .OrderBy(p => p.Value);

                return CompletionResult.FromHintOptions(options, Loc.GetString($"cmd-forcemap-hint"));
            }

            return CompletionResult.Empty;
        }
    }
}
