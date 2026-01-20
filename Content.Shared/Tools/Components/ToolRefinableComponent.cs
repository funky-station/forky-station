// SPDX-FileCopyrightText: 2024 Verm
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Content.Shared.Storage;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.Tools.Systems;

namespace Content.Shared.Tools.Components;

/// <summary>
/// Used for something that can be refined by welder.
/// For example, glass shard can be refined to glass sheet.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(ToolRefinablSystem))]
public sealed partial class ToolRefinableComponent : Component
{
    /// <summary>
    /// The items created when the item is refined.
    /// </summary>
    [DataField(required: true)]
    public List<EntitySpawnEntry> RefineResult = new();

    /// <summary>
    /// The amount of time it takes to refine a given item.
    /// </summary>
    [DataField]
    public float RefineTime = 2f;

    /// <summary>
    /// The amount of fuel it takes to refine a given item.
    /// </summary>
    [DataField]
    public float RefineFuel = 3f;

    /// <summary>
    /// The tool type needed in order to refine this item.
    /// </summary>
    [DataField]
    public ProtoId<ToolQualityPrototype> QualityNeeded = "Welding";
}
