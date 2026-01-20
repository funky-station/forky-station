// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

namespace Content.Server.NPC.Pathfinding;

public enum PathResult : byte
{
    NoPath,
    PartialPath,
    Path,
    Continuing,
}
