// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 moonheart08
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.RoundEnd;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.GameTicking.Commands;

[AdminCommand(AdminFlags.Round)]
public sealed class RestartRoundCommand : LocalizedEntityCommands
{
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;

    public override string Command => "restartround";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (_gameTicker.RunLevel != GameRunLevel.InRound)
        {
            shell.WriteLine(Loc.GetString("shell-can-only-run-while-round-is-active"));
            return;
        }

        _roundEndSystem.EndRound();
    }
}

[AdminCommand(AdminFlags.Round)]
public sealed class RestartRoundNowCommand : LocalizedEntityCommands
{
    [Dependency] private readonly GameTicker _gameTicker = default!;

    public override string Command => "restartroundnow";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        _gameTicker.RestartRound();
    }
}
