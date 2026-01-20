// SPDX-FileCopyrightText: 2024 Tornado Tech
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

namespace Content.Server.NPC.HTN.Preconditions;

/// <summary>
/// Checks for the presence of the value by the specified <see cref="KeyExistsPrecondition.Key"/> in the <see cref="NPCBlackboard"/>.
/// Returns true if there is a value.
/// </summary>
public sealed partial class KeyExistsPrecondition : HTNPrecondition
{
    [DataField(required: true), ViewVariables]
    public string Key = string.Empty;

    public override bool IsMet(NPCBlackboard blackboard)
    {
        return blackboard.ContainsKey(Key);
    }
}
