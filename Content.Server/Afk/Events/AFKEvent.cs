// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Robust.Shared.Player;

namespace Content.Server.Afk.Events;

/// <summary>
/// Raised whenever a player goes afk.
/// </summary>
[ByRefEvent]
public readonly struct AFKEvent
{
    public readonly ICommonSession Session;

    public AFKEvent(ICommonSession playerSession)
    {
        Session = playerSession;
    }
}
