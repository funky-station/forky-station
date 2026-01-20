// SPDX-FileCopyrightText: 2024 Krunklehorn
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 AJCM-git
// SPDX-FileCopyrightText: 2022 ike709
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Pointing.Components;
using System.Numerics;

namespace Content.Client.Pointing.Components;
[RegisterComponent]
public sealed partial class PointingArrowComponent : SharedPointingArrowComponent
{
    /// <summary>
    /// How far the arrow moves up and down during the floating phase.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("offset")]
    public Vector2 Offset = new(0, 0.25f);

    public readonly string AnimationKey = "pointingarrow";
}
