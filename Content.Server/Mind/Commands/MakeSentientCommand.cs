// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 ShadowCommander
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Mind.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed class MakeSentientCommand : LocalizedEntityCommands
{
    [Dependency] private readonly MindSystem _mindSystem = default!;

    public override string Command => "makesentient";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteLine(Loc.GetString("shell-need-exactly-one-argument"));
            return;
        }

        if (!NetEntity.TryParse(args[0], out var entNet) || !EntityManager.TryGetEntity(entNet, out var entId) || !EntityManager.EntityExists(entId))
        {
            shell.WriteLine(Loc.GetString("shell-could-not-find-entity-with-uid", ("uid", args[0])));
            return;
        }

        _mindSystem.MakeSentient(entId.Value);
    }
}
