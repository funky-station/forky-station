// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-License-Identifier: MIT

namespace Content.Server.GameTicking.Events;

/// <summary>
///     Raised at the start of <see cref="GameTicker.StartRound"/>, after round id has been incremented
/// </summary>
public sealed class RoundStartingEvent : EntityEventArgs
{
    public RoundStartingEvent(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
