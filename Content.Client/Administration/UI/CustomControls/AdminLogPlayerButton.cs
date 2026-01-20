// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-License-Identifier: MIT

using System;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Administration.UI.CustomControls;

public sealed class AdminLogPlayerButton : Button
{
    public AdminLogPlayerButton(Guid id)
    {
        Id = id;
        ClipText = true;
        ToggleMode = true;
    }

    public Guid Id { get; }
}
