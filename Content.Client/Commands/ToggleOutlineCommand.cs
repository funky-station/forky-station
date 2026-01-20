// SPDX-FileCopyrightText: 2024 PrPleGoo
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 F77F
// SPDX-License-Identifier: MIT

using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;

namespace Content.Client.Commands;

[AnyCommand]
public sealed class ToggleOutlineCommand : LocalizedCommands
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;

    public override string Command => "toggleoutline";

    public override string Help => LocalizationManager.GetString($"cmd-{Command}-help", ("command", Command));

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var cvar = CCVars.OutlineEnabled;
        var old = _configurationManager.GetCVar(cvar);

        _configurationManager.SetCVar(cvar, !old);
        shell.WriteLine(LocalizationManager.GetString($"cmd-{Command}-notify", ("state", _configurationManager.GetCVar(cvar))));
    }
}
