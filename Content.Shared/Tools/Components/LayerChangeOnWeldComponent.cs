// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Rane
// SPDX-License-Identifier: MIT

using Content.Shared.Physics;
using Content.Shared.Tools.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Tools.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(WeldableSystem))]
public sealed partial class LayerChangeOnWeldComponent : Component
{
    [DataField("unWeldedLayer")]
    [ViewVariables]
    public CollisionGroup UnWeldedLayer = CollisionGroup.AirlockLayer;

    [DataField("weldedLayer")]
    [ViewVariables]
    public CollisionGroup WeldedLayer = CollisionGroup.WallLayer;
}
