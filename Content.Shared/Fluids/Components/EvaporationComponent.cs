// SPDX-FileCopyrightText: 2025 Perry Fraser
// SPDX-FileCopyrightText: 2025 Jajsha
// SPDX-FileCopyrightText: 2025 Partmedia
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Willhelm53
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-License-Identifier: MIT

using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Fluids.Components;

/// <summary>
/// Added to puddles that contain water so it may evaporate over time.
/// </summary>
[NetworkedComponent, AutoGenerateComponentPause, AutoGenerateComponentState]
[RegisterComponent, Access(typeof(SharedPuddleSystem))]
public sealed partial class EvaporationComponent : Component
{
    /// <summary>
    /// The next time we remove the EvaporationSystem reagent amount from this entity.
    /// </summary>
    [AutoNetworkedField, AutoPausedField]
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextTick;

    /// <summary>
    /// Evaporation factor. Multiplied by the evaporating speed of the reagent.
    /// </summary>
    [DataField]
    public FixedPoint2 EvaporationAmount = FixedPoint2.New(1);

    /// <summary>
    /// The effect spawned when the puddle fully evaporates.
    /// </summary>
    [DataField]
    public EntProtoId EvaporationEffect = "PuddleSparkle";
}
