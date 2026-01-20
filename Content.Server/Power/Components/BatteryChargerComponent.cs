// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Server.Power.NodeGroups;
using Content.Shared.Power.Components;

namespace Content.Server.Power.Components
{
    /// <summary>
    ///     Connects the loading side of a <see cref="BatteryComponent"/> to a non-APC power network.
    /// </summary>
    [RegisterComponent]
    public sealed partial class BatteryChargerComponent : BasePowerNetComponent
    {
        protected override void AddSelfToNet(IPowerNet net)
        {
            net.AddCharger(this);
        }

        protected override void RemoveSelfFromNet(IPowerNet net)
        {
            net.RemoveCharger(this);
        }
    }
}
