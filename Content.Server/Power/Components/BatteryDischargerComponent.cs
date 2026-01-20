// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 py01
// SPDX-License-Identifier: MIT

using Content.Server.Power.NodeGroups;

namespace Content.Server.Power.Components
{
    [RegisterComponent]
    public sealed partial class BatteryDischargerComponent : BasePowerNetComponent
    {
        protected override void AddSelfToNet(IPowerNet net)
        {
            net.AddDischarger(this);
        }

        protected override void RemoveSelfFromNet(IPowerNet net)
        {
            net.RemoveDischarger(this);
        }
    }
}
