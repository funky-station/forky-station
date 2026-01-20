// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Centronias
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Server.NPC.Queries.Queries;

public sealed partial class ComponentFilter : UtilityQueryFilter
{
    /// <summary>
    /// Components to filter for.
    /// </summary>
    [DataField("components", required: true)]
    public ComponentRegistry Components = new();

    /// <summary>
    /// If true, this filter retains entities with ALL of the specified components. If false, this filter removes
    /// entities with ANY of the specified components.
    /// </summary>
    [DataField]
    public bool RetainWithComp = true;
}
