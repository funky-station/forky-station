// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2024 Brandon Hu
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.EUI;
using Content.Server.NPC.UI;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.NPC.Commands;

[AdminCommand(AdminFlags.Debug)]
public sealed class NpcCommand : LocalizedEntityCommands
{
    [Dependency] private readonly EuiManager _euiManager = default!;

    public override string Command => "npc";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (shell.Player is not { } playerSession)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        _euiManager.OpenEui(new NPCEui(), playerSession);
    }
}
