// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-License-Identifier: MIT

using Robust.Shared.Player;

namespace Content.Server.Ghost.Roles.Components;

[ByRefEvent]
public record struct TakeGhostRoleEvent(ICommonSession Player)
{
    public bool TookRole { get; set; }
}
