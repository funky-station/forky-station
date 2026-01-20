// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Julian Giebel
// SPDX-License-Identifier: MIT

using Content.Server.DeviceNetwork.Systems.Devices;

namespace Content.Server.DeviceNetwork.Components.Devices
{
    [RegisterComponent]
    [Access(typeof(ApcNetSwitchSystem))]
    public sealed partial class ApcNetSwitchComponent : Component
    {
        [ViewVariables] public bool State;
    }
}
