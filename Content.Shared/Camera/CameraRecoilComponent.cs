// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using System.Numerics;
using Robust.Shared.GameStates;

namespace Content.Shared.Camera;

[RegisterComponent]
[NetworkedComponent]
public sealed partial class CameraRecoilComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    public Vector2 CurrentKick { get; set; }

    [ViewVariables(VVAccess.ReadWrite)]
    public Vector2 LastKick { get; set; }
    
    [ViewVariables(VVAccess.ReadWrite)]
    public float LastKickTime { get; set; }

    /// <summary>
    ///     Basically I needed a way to chain this effect for the attack lunge animation. Sorry!
    /// </summary>
    ///
    [ViewVariables(VVAccess.ReadWrite)]
    public Vector2 BaseOffset { get; set; }
}
