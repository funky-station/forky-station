// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.NPC;

/// <summary>
/// Has debug information for HTN NPCs.
/// </summary>
[Serializable, NetSerializable]
public sealed class HTNMessage : EntityEventArgs
{
    public NetEntity Uid;
    public string Text = string.Empty;
}
