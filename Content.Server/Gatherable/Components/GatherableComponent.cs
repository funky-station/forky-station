// SPDX-FileCopyrightText: 2025 Red
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Chris V
// SPDX-License-Identifier: MIT

using Content.Shared.EntityTable.EntitySelectors;
using Content.Shared.Whitelist;

namespace Content.Server.Gatherable.Components;

[RegisterComponent]
[Access(typeof(GatherableSystem))]
public sealed partial class GatherableComponent : Component
{
    /// <summary>
    ///     Whitelist for specifying the kind of tools can be used on a resource
    ///     Supports multiple tags.
    /// </summary>
    [DataField(required: true)]
    public EntityWhitelist? ToolWhitelist;

    /// <summary>
    ///     YAML example below
    ///     (Tag1, Tag2, LootTableID1, LootTableID2 are placeholders for example)
    ///     --------------------
    ///     useMappedLoot: true
    ///     toolWhitelist:
    ///       tags:
    ///        - Tag1
    ///        - Tag2
    ///     loot:
    ///       Tag1: !type:NestedSelector
    ///         tableId: LootTableID1
    ///       Tag2: !type:NestedSelector
    ///         tableId: LootTableID2
    /// </summary>
    [DataField]
    public Dictionary<string, EntityTableSelector>? Loot = new();

    /// <summary>
    /// Random shift of the appearing entity during gathering
    /// </summary>
    [DataField]
    public float GatherOffset = 0.3f;
}
