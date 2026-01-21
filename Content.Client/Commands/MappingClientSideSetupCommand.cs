// SPDX-FileCopyrightText: 2022 20kdc <asdd2808@gmail.com>
// SPDX-FileCopyrightText: 2024 Mervill <mervills.email@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 PrPleGoo <PrPleGoo@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 SpaceManiac <tad@platymuus.com>
// SPDX-License-Identifier: MIT

using Content.Client.Actions;
using Content.Client.Markers;
using Content.Client.SubFloor;
using Robust.Client.Graphics;
using Robust.Shared.Console;

namespace Content.Client.Commands;

internal sealed class MappingClientSideSetupCommand : LocalizedEntityCommands
{
    [Dependency] private readonly ILightManager _lightManager = default!;
    [Dependency] private readonly ActionsSystem _actionSystem = default!;
    [Dependency] private readonly MarkerSystem _markerSystem = default!;
    [Dependency] private readonly SubFloorHideSystem _subfloorSystem = default!;

    public override string Command => "mappingclientsidesetup";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (_lightManager.LockConsoleAccess)
            return;

        _markerSystem.MarkersVisible = true;
        _lightManager.Enabled = false;
        _subfloorSystem.ShowAll = true;
        _actionSystem.LoadActionAssignments("/mapping_actions.yml", false);
    }
}

