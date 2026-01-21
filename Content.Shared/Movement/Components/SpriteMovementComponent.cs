// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components;

/// <summary>
/// Updates a sprite layer based on whether an entity is moving via input or not.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class SpriteMovementComponent : Component
{
    /// <summary>
    /// Layer and sprite state to use when moving.
    /// </summary>
    [DataField]
    public Dictionary<string, PrototypeLayerData> MovementLayers = new();

    /// <summary>
    /// Layer and sprite state to use when not moving.
    /// </summary>
    [DataField]
    public Dictionary<string, PrototypeLayerData> NoMovementLayers = new();

    [DataField, AutoNetworkedField]
    public bool IsMoving;
}
