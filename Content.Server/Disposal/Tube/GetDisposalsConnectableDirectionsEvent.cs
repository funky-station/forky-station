// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-License-Identifier: MIT

namespace Content.Server.Disposal.Tube;

[ByRefEvent]
public record struct GetDisposalsConnectableDirectionsEvent
{
    public Direction[] Connectable;
}
