// SPDX-FileCopyrightText: 2025 Perry Fraser
// SPDX-FileCopyrightText: 2025 Leon Friedrich
// SPDX-FileCopyrightText: 2024 IProduceWidgets
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Emisse
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry.EntitySystems;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Chemistry.Components;

/// <summary>
/// Passively increases a solution's quantity of a reagent.
/// </summary>
[RegisterComponent, AutoGenerateComponentPause, AutoGenerateComponentState, NetworkedComponent]
[Access(typeof(SolutionRegenerationSystem))]
public sealed partial class SolutionRegenerationComponent : Component
{
    /// <summary>
    /// The name of the solution to add to.
    /// </summary>
    [DataField("solution", required: true)]
    public string SolutionName = string.Empty;

    /// <summary>
    /// The solution to add reagents to.
    /// </summary>
    [ViewVariables]
    public Entity<SolutionComponent>? SolutionRef = null;

    /// <summary>
    /// The reagent(s) to be regenerated in the solution.
    /// </summary>
    [DataField(required: true)]
    public Solution Generated = default!;

    /// <summary>
    /// How long it takes to regenerate once.
    /// </summary>
    [DataField]
    public TimeSpan Duration = TimeSpan.FromSeconds(1);

    /// <summary>
    /// The time when the next regeneration will occur.
    /// </summary>
    [DataField("nextChargeTime", customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField, AutoNetworkedField]
    public TimeSpan NextRegenTime;
}
