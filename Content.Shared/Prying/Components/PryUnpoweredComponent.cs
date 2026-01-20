// SPDX-FileCopyrightText: 2023 nikthechampiongr
// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Prying.Components;

///<summary>
/// Applied to entities that can be pried open without tools while unpowered
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PryUnpoweredComponent : Component
{
    [DataField]
    public float PryModifier = 0.1f;
}
