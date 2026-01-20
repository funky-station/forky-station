// SPDX-FileCopyrightText: 2025 IProduceWidgets
// SPDX-FileCopyrightText: 2025 UpAndLeaves
// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Shared.NodeContainer.NodeGroups;
using Robust.Shared.Serialization;

namespace Content.Shared.Power
{
    [Serializable, NetSerializable]
    public enum ChargeState : byte
    {
        Still = 0,
        Charging = 1,
        Discharging = 2,
    }

    [Serializable, NetSerializable]
    public enum PowerWireActionKey : byte
    {
        Key,
        Status,
        Pulsed,
        Electrified,
        PulseCancel,
        ElectrifiedCancel,
        MainWire,
        WireCount,
        CutWires
    }

    [Serializable, NetSerializable]
    public enum CableType
    {
        HighVoltage,
        MediumVoltage,
        Apc,
        ExCable
    }

    [Serializable, NetSerializable]
    public enum Voltage
    {
        High = NodeGroupID.HVPower,
        Medium = NodeGroupID.MVPower,
        Apc = NodeGroupID.Apc,
    }
}
