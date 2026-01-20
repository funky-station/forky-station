// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Map;

namespace Content.Server.Movement.Components;

[RegisterComponent]
public sealed partial class LagCompensationComponent : Component
{
    [ViewVariables]
    public readonly Queue<ValueTuple<TimeSpan, EntityCoordinates, Angle>> Positions = new();
}
