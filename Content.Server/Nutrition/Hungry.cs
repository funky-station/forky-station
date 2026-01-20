// SPDX-FileCopyrightText: 2024 Mervill
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Console;

namespace Content.Server.Nutrition;

[AdminCommand(AdminFlags.Debug)]
public sealed class Hungry : LocalizedEntityCommands
{
    public override string Command => "hungry";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player == null)
        {
            shell.WriteError(Loc.GetString("cmd-nutrition-error-player"));
            return;
        }

        if (player.AttachedEntity is not {Valid: true} playerEntity)
        {
            shell.WriteError(Loc.GetString("cmd-nutrition-error-entity"));
            return;
        }

        if (!EntityManager.TryGetComponent(playerEntity, out HungerComponent? hunger))
        {
            shell.WriteError(Loc.GetString("cmd-nutrition-error-component", ("comp", nameof(HungerComponent))));
            return;
        }

        var hungryThreshold = hunger.Thresholds[HungerThreshold.Starving];
        EntityManager.System<HungerSystem>().SetHunger(playerEntity, hungryThreshold, hunger);
    }
}
