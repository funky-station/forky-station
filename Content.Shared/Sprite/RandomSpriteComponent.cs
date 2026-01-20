// SPDX-FileCopyrightText: 2024 deathride58
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Sprite;

[RegisterComponent, NetworkedComponent]
public sealed partial class RandomSpriteComponent : Component
{
    /// <summary>
    /// Whether or not all groups from <see cref="Available"/> are used,
    /// or if only one is picked at random.
    /// </summary>
    [DataField("getAllGroups")]
    public bool GetAllGroups;

    /// <summary>
    /// Available colors based on group, parsed layer enum, state, and color.
    /// Stored as a list so we can have groups of random sprites (e.g. tech_base + tech_flare for holoparasite)
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("available")]
    public List<Dictionary<string, Dictionary<string, string?>>> Available = new();

    /// <summary>
    /// Selected colors
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("selected")]
    public Dictionary<string, (string State, Color? Color)> Selected = new();
}
