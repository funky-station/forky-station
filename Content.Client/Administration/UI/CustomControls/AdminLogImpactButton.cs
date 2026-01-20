// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-License-Identifier: MIT

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.CustomControls;

public sealed class AdminLogImpactButton : Button
{
    public AdminLogImpactButton(LogImpact impact)
    {
        Impact = impact;
        ToggleMode = true;
        Pressed = true;
    }

    public LogImpact Impact { get; }
}
