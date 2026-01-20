// SPDX-FileCopyrightText: 2025 chromiumboy
// SPDX-FileCopyrightText: 2025 Ertanic
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2023 PixelTK
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 08A
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Morb
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Kara D
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2020 Swept
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-License-Identifier: MIT

using Content.Shared.Construction.Conditions;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Shared.Construction.Prototypes;

[Prototype]
public sealed partial class ConstructionPrototype : IPrototype
{
    [DataField("conditions")] private List<IConstructionCondition> _conditions = new();

    /// <summary>
    ///     Hide from the construction list
    /// </summary>
    [DataField]
    public bool Hide = false;

    /// <summary>
    ///     Friendly name displayed in the construction GUI.
    /// </summary>
    [DataField("name")]
    public LocId? SetName;

    public string? Name;

    /// <summary>
    ///     "Useful" description displayed in the construction GUI.
    /// </summary>
    [DataField("description")]
    public LocId? SetDescription;

    public string? Description;

    /// <summary>
    ///     The <see cref="ConstructionGraphPrototype"/> this construction will be using.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<ConstructionGraphPrototype> Graph { get; private set; } = string.Empty;

    /// <summary>
    ///     The target <see cref="ConstructionGraphNode"/> this construction will guide the user to.
    /// </summary>
    [DataField(required: true)]
    public string TargetNode { get; private set; } = default!;

    /// <summary>
    ///     The starting <see cref="ConstructionGraphNode"/> this construction will start at.
    /// </summary>
    [DataField(required: true)]
    public string StartNode { get; private set; } = default!;

    /// <summary>
    ///     If you can start building or complete steps on impassable terrain.
    /// </summary>
    [DataField]
    public bool CanBuildInImpassable { get; private set; }

    /// <summary>
    /// If not null, then this is used to check if the entity trying to construct this is whitelisted.
    /// If they're not whitelisted, hide the item.
    /// </summary>
    [DataField]
    public EntityWhitelist? EntityWhitelist { get; private set; }

    [DataField] public string Category { get; private set; } = string.Empty;

    [DataField("objectType")] public ConstructionType Type { get; private set; } = ConstructionType.Structure;

    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public string PlacementMode = "PlaceFree";

    /// <summary>
    ///     Whether this construction can be constructed rotated or not.
    /// </summary>
    [DataField]
    public bool CanRotate = true;

    /// <summary>
    ///     Construction to replace this construction with when the current one is 'flipped'
    /// </summary>
    [DataField]
    public ProtoId<ConstructionPrototype>? Mirror { get; private set; }

    /// <summary>
    ///     Possible constructions to replace this one with as determined by the placement mode
    /// </summary>
    [DataField]
    public ProtoId<ConstructionPrototype>[] AlternativePrototypes = [];

    public IReadOnlyList<IConstructionCondition> Conditions => _conditions;
}

public enum ConstructionType
{
    Structure,
    Item,
}
