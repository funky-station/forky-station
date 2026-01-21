// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Client.NPC.HTN;
using Robust.Shared.Console;

namespace Content.Client.NPC;

public sealed class ShowHtnCommand : LocalizedEntityCommands
{
    [Dependency] private readonly HTNSystem _htnSystem = default!;

    public override string Command => "showhtn";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        _htnSystem.EnableOverlay ^= true;
    }
}
