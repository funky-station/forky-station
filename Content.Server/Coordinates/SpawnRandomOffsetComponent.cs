// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

namespace Content.Server.Coordinates;

[RegisterComponent]
public sealed partial class SpawnRandomOffsetComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("offset")] public float Offset = 0.5f;
}
