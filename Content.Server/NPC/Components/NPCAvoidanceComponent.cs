// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

namespace Content.Server.NPC.Components;

/// <summary>
/// Should this entity be considered for collision avoidance
/// </summary>
[RegisterComponent]
public sealed partial class NPCAvoidanceComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("enabled")]
    public bool Enabled = true;
}
