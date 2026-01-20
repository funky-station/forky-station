// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using System.Numerics;
using Robust.Shared.GameStates;

namespace Content.Shared.Placeable;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(PlaceableSurfaceSystem))]
public sealed partial class PlaceableSurfaceComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool IsPlaceable { get; set; } = true;

    [DataField, AutoNetworkedField]
    public bool PlaceCentered { get; set; }

    [DataField, AutoNetworkedField]
    public Vector2 PositionOffset { get; set; }
}
