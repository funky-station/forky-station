// SPDX-FileCopyrightText: 2024 0x6273
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Content.Server.Body.Systems;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Body.Components;

[RegisterComponent]
[Access(typeof(ThermalRegulatorSystem))]
public sealed partial class ThermalRegulatorComponent : Component
{
    /// <summary>
    /// The next time that the body will regulate its heat.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextUpdate;

    /// <summary>
    /// The interval at which thermal regulation is processed.
    /// </summary>
    [DataField]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Heat generated due to metabolism. It's generated via metabolism
    /// </summary>
    [DataField]
    public float MetabolismHeat;

    /// <summary>
    /// Heat output via radiation.
    /// </summary>
    [DataField]
    public float RadiatedHeat;

    /// <summary>
    /// Maximum heat regulated via sweat
    /// </summary>
    [DataField]
    public float SweatHeatRegulation;

    /// <summary>
    /// Maximum heat regulated via shivering
    /// </summary>
    [DataField]
    public float ShiveringHeatRegulation;

    /// <summary>
    /// Amount of heat regulation that represents thermal regulation processes not
    /// explicitly coded.
    /// </summary>
    [DataField]
    public float ImplicitHeatRegulation;

    /// <summary>
    /// Normal body temperature
    /// </summary>
    [DataField]
    public float NormalBodyTemperature;

    /// <summary>
    /// Deviation from normal temperature for body to start thermal regulation
    /// </summary>
    [DataField]
    public float ThermalRegulationTemperatureThreshold;
}
