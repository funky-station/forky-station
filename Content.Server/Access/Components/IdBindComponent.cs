// SPDX-FileCopyrightText: 2025 Verm <32827189+Vermidia@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Server.Access.Components;

[RegisterComponent]
public sealed partial class IdBindComponent : Component
{
    /// <summary>
    /// If true, also tries to get the PDA and set the owner to the entity
    /// </summary>
    [DataField]
    public bool BindPDAOwner = true;
}

