// SPDX-FileCopyrightText: 2024 IProduceWidgets
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Dawid Bla
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 py01
// SPDX-License-Identifier: MIT

using Content.Server.Power.EntitySystems;
using Content.Shared.Power;
using Content.Shared.Tools;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System.Diagnostics.Tracing;
using Content.Shared.Tools.Systems;

namespace Content.Server.Power.Components;

/// <summary>
///     Allows the attached entity to be destroyed by a cutting tool, dropping a piece of cable.
/// </summary>
[RegisterComponent]
[Access(typeof(CableSystem))]
public sealed partial class CableComponent : Component
{
    [DataField]
    public EntProtoId CableDroppedOnCutPrototype = "CableHVStack1";

    /// <summary>
    /// The tool quality needed to cut the cable. Setting to null prevents cutting.
    /// </summary>
    [DataField]
    public ProtoId<ToolQualityPrototype>? CuttingQuality = SharedToolSystem.CutQuality;

    /// <summary>
    ///     Checked by <see cref="CablePlacerComponent"/> to determine if there is
    ///     already a cable of a type on a tile.
    /// </summary>
    [DataField("cableType")]
    public CableType CableType = CableType.HighVoltage;

    [DataField("cuttingDelay")]
    public float CuttingDelay = 1f;
}

/// <summary>
///     Event to be raised when a cable is anchored / unanchored
/// </summary>
[ByRefEvent]
public readonly struct CableAnchorStateChangedEvent
{
    public readonly TransformComponent Transform;
    public EntityUid Entity => Transform.Owner;
    public bool Anchored => Transform.Anchored;

    /// <summary>
    ///     If true, the entity is being detached to null-space
    /// </summary>
    public readonly bool Detaching;

    public CableAnchorStateChangedEvent(TransformComponent transform, bool detaching = false)
    {
        Detaching = detaching;
        Transform = transform;
    }
}
