// SPDX-FileCopyrightText: 2025 ArtisticRoomba
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 py01
// SPDX-License-Identifier: MIT

using Content.Server.Power.NodeGroups;
using Content.Server.Power.Pow3r;
using Content.Shared.Guidebook;

namespace Content.Server.Power.Components
{
    [RegisterComponent]
    public sealed partial class PowerSupplierComponent : BaseNetConnectorComponent<IBasePowerNet>
    {
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("supplyRate")]
        [GuidebookData]
        public float MaxSupply { get => NetworkSupply.MaxSupply; set => NetworkSupply.MaxSupply = value; }

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("supplyRampTolerance")]
        public float SupplyRampTolerance
        {
            get => NetworkSupply.SupplyRampTolerance;
            set => NetworkSupply.SupplyRampTolerance = value;
        }

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("supplyRampRate")]
        public float SupplyRampRate
        {
            get => NetworkSupply.SupplyRampRate;
            set => NetworkSupply.SupplyRampRate = value;
        }

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("supplyRampPosition")]
        public float SupplyRampPosition
        {
            get => NetworkSupply.SupplyRampPosition;
            set => NetworkSupply.SupplyRampPosition = value;
        }

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("enabled")]
        public bool Enabled
        {
            get => NetworkSupply.Enabled;
            set => NetworkSupply.Enabled = value;
        }

        [ViewVariables] public float CurrentSupply => NetworkSupply.CurrentSupply;

        [ViewVariables]
        public PowerState.Supply NetworkSupply { get; } = new();

        protected override void AddSelfToNet(IBasePowerNet powerNet)
        {
            powerNet.AddSupplier(this);
        }

        protected override void RemoveSelfFromNet(IBasePowerNet powerNet)
        {
            powerNet.RemoveSupplier(this);
        }
    }
}
