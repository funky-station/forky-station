// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using System.Numerics;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared.Physics;

/// <summary>
/// Just draws a generic line between this entity and the target.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class JointVisualsComponent : Component
{
    /// <summary>
    /// The sprite to use for the line.
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public SpriteSpecifier Sprite = default!;

    /// <summary>
    /// The line is drawn between this target and the entity owning the component.
    /// </summary>
    /// <summary>
    /// TODO: WeakEntityReference.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Target;

    /// <summary>
    /// Offset from Body A.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Vector2 OffsetA;

    /// <summary>
    /// Offset from Body B.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Vector2 OffsetB;
}
