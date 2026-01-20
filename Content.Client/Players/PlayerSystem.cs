// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Shared.Players;
using Robust.Shared.Player;

namespace Content.Client.Players;

public sealed class PlayerSystem : SharedPlayerSystem
{
    public override ContentPlayerData? ContentData(ICommonSession? session)
    {
        return null;
    }
}
