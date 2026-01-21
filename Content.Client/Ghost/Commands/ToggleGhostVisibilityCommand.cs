// SPDX-FileCopyrightText: 2024 ShadowCommander <shadowjjt@gmail.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Console;

namespace Content.Client.Ghost.Commands;

public sealed class ToggleGhostVisibilityCommand : LocalizedEntityCommands
{
    [Dependency] private readonly GhostSystem _ghost = default!;

    public override string Command => "toggleghostvisibility";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 0 && bool.TryParse(args[0], out var visibility))
            _ghost.ToggleGhostVisibility(visibility);
        else
            _ghost.ToggleGhostVisibility();
    }
}
