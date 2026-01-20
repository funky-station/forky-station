// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2022 themias
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-License-Identifier: MIT

using Content.Shared.Nutrition.EntitySystems;

namespace Content.Shared.Nutrition.Components;

/// <summary>
///     Component that denotes a piece of clothing that blocks the mouth or otherwise prevents eating & drinking.
/// </summary>
/// <remarks>
///     In the event that more head-wear & mask functionality is added (like identity systems, or raising/lowering of
///     masks), then this component might become redundant.
/// </remarks>
[RegisterComponent, Access(typeof(IngestionSystem))]
public sealed partial class IngestionBlockerComponent : Component
{
    /// <summary>
    ///     Is this component currently blocking consumption.
    /// </summary>
    [DataField]
    public bool Enabled { get; set; } = true;
}
