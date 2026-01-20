// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2025 SpaceManiac
// SPDX-FileCopyrightText: 2024 PrPleGoo
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-License-Identifier: MIT

using Content.Client.Markers;
using Content.Client.Popups;
using Content.Client.SubFloor;
using Robust.Shared.Console;

namespace Content.Client.Commands;

internal sealed class ShowMarkersCommand : LocalizedEntityCommands
{
    [Dependency] private readonly MarkerSystem _markerSystem = default!;

    public override string Command => "showmarkers";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        _markerSystem.MarkersVisible ^= true;
    }
}

internal sealed class ShowSubFloor : LocalizedEntityCommands
{
    [Dependency] private readonly SubFloorHideSystem _subfloorSystem = default!;

    public override string Command => "showsubfloor";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        _subfloorSystem.ShowAll ^= true;
    }
}

internal sealed class NotifyCommand : LocalizedEntityCommands
{
    [Dependency] private readonly PopupSystem _popupSystem = default!;

    public override string Command => "notify";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        _popupSystem.PopupCursor(args[0]);
    }
}
