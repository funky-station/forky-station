// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 py01
// SPDX-License-Identifier: MIT

using Content.Server.Power.NodeGroups;

namespace Content.Server.Power.Components
{
    public interface IBasePowerNetComponent : IBaseNetConnectorComponent<IPowerNet>
    {

    }

    public abstract partial class BasePowerNetComponent : BaseNetConnectorComponent<IPowerNet>
    {
    }
}
