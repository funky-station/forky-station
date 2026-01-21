// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Light.Components;

/// <summary>
/// Can activate <see cref="LightOnCollideComponent"/> when collided with.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class LightOnCollideColliderComponent : Component
{
    [DataField]
    public string FixtureId = "lightTrigger";
}
