// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

namespace Content.Server.NPC.Systems;

/// <summary>
/// Handles sight + sounds for NPCs.
/// </summary>
public sealed partial class NPCPerceptionSystem : EntitySystem
{
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        UpdateRecentlyInjected(frameTime);
    }
}
