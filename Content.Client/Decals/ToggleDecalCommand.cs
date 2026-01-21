// SPDX-FileCopyrightText: 2021 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Console;

namespace Content.Client.Decals;

public sealed class ToggleDecalCommand : LocalizedEntityCommands
{
    [Dependency] private readonly DecalSystem _decal = default!;

    public override string Command => "toggledecals";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        _decal.ToggleOverlay();
    }
}
