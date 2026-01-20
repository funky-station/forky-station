// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 themias
// SPDX-FileCopyrightText: 2023 Moony
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 py01
// SPDX-FileCopyrightText: 2020 nuke
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 Injazz
// SPDX-License-Identifier: MIT

using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.Components;

/// <summary>
/// A container that transforms its appearance depending on the reagent it contains.
/// It returns to its initial state once the reagent is removed.
/// e.g. An empty glass changes to a beer glass when beer is added to it.
///
/// Should probably be joined with SolutionContainerVisualsComponent when solutions are networked.
/// </summary>
[RegisterComponent, Access(typeof(TransformableContainerSystem))]
public sealed partial class TransformableContainerComponent : Component
{
    /// <summary>
    /// This is the initial metadata description for the container.
    /// It will revert to this when emptied.
    ///     /// It defaults to the description of the parent entity unless overwritten.
    /// </summary>
    [DataField]
    public string? InitialDescription;

    /// <summary>
    /// This stores whatever primary reagent is currently in the container.
    /// It is used to help determine if a transformation is needed on solution update.
    /// </summary>
    [DataField]
    public ProtoId<ReagentPrototype>? CurrentReagent;

    /// <summary>
    /// This returns whether this container in a transformed or initial state.
    /// </summary>
    [DataField]
    public bool Transformed;
}
