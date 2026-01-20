// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.GameTicking.Commands;

[AdminCommand(AdminFlags.Round)]
public sealed class DelayStartCommand : LocalizedEntityCommands
{
    [Dependency] private readonly GameTicker _gameTicker = default!;

    public override string Command => "delaystart";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (_gameTicker.RunLevel != GameRunLevel.PreRoundLobby)
        {
            shell.WriteLine(Loc.GetString("shell-can-only-run-from-pre-round-lobby"));
            return;
        }

        switch (args.Length)
        {
            case 0:
                var paused = _gameTicker.TogglePause();
                shell.WriteLine(Loc.GetString(paused ? "cmd-delaystart-paused" : "cmd-delaystart-unpaused"));
                return;
            case 1:
                break;
            default:
                shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
                return;
        }

        if (!uint.TryParse(args[0], out var seconds) || seconds == 0)
        {
            shell.WriteLine(Loc.GetString("cmd-delaystart-invalid-seconds", ("value", args[0])));
            return;
        }

        var time = TimeSpan.FromSeconds(seconds);
        if (!_gameTicker.DelayStart(time))
            shell.WriteLine(Loc.GetString("cmd-delaystart-too-late"));
    }
}
