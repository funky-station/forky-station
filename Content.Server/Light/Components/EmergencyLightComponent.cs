// SPDX-FileCopyrightText: 2024 superjj18
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2021 py01
// SPDX-FileCopyrightText: 2021 Daniel Castro Razo
// SPDX-FileCopyrightText: 2020 Julian Giebel
// SPDX-License-Identifier: MIT

using Content.Server.Light.EntitySystems;
using Content.Shared.Light.Components;

namespace Content.Server.Light.Components;

/// <summary>
///     Component that represents an emergency light, it has an internal battery that charges when the power is on.
/// </summary>
[RegisterComponent, Access(typeof(EmergencyLightSystem))]
public sealed partial class EmergencyLightComponent : SharedEmergencyLightComponent
{
    [ViewVariables]
    public EmergencyLightState State;

    /// <summary>
    ///     Is this emergency light forced on for some reason and cannot be disabled through normal means
    ///     (i.e. blue alert or higher?)
    /// </summary>
    public bool ForciblyEnabled = false;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("wattage")]
    public float Wattage = 5;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("chargingWattage")]
    public float ChargingWattage = 60;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("chargingEfficiency")]
    public float ChargingEfficiency = 0.85f;

    public Dictionary<EmergencyLightState, string> BatteryStateText = new()
    {
        { EmergencyLightState.Full, "emergency-light-component-light-state-full" },
        { EmergencyLightState.Empty, "emergency-light-component-light-state-empty" },
        { EmergencyLightState.Charging, "emergency-light-component-light-state-charging" },
        { EmergencyLightState.On, "emergency-light-component-light-state-on" }
    };
}

public enum EmergencyLightState : byte
{
    Charging,
    Full,
    Empty,
    On
}

public sealed class EmergencyLightEvent : EntityEventArgs
{
    public EmergencyLightState State { get; }

    public EmergencyLightEvent(EmergencyLightState state)
    {
        State = state;
    }
}
