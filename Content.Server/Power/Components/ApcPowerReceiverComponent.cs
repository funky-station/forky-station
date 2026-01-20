// SPDX-FileCopyrightText: 2025 chromiumboy
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 ShadowCommander
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Kevin Zheng
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 Julian Giebel
// SPDX-FileCopyrightText: 2021 Julian Giebel
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2021 py01
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Kmc2000
// SPDX-License-Identifier: MIT

using Content.Server.Power.NodeGroups;
using Content.Server.Power.Pow3r;
using Content.Shared.Power.Components;

namespace Content.Server.Power.Components
{
    /// <summary>
    ///     Attempts to link with a nearby <see cref="ApcPowerProviderComponent"/>s
    ///     so that it can receive power from a <see cref="IApcNet"/>.
    /// </summary>
    [RegisterComponent]
    public sealed partial class ApcPowerReceiverComponent : SharedApcPowerReceiverComponent
    {
        /// <summary>
        ///     Amount of charge this needs from an APC per second to function.
        /// </summary>
        [DataField("powerLoad")]
        public override float Load
        {
            get => NetworkLoad.DesiredPower;
            set => NetworkLoad.DesiredPower = value;
        }

        public ApcPowerProviderComponent? Provider = null;

        /// <summary>
        ///     When false, causes this to appear powered even if not receiving power from an Apc.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        public override bool NeedsPower
        {
            get => _needsPower;
            set
            {
                _needsPower = value;
            }
        }

        [DataField("needsPower")]
        private bool _needsPower = true;

        /// <summary>
        ///     When true, causes this to never appear powered.
        /// </summary>
        [DataField("powerDisabled")]
        public override bool PowerDisabled
        {
            get => !NetworkLoad.Enabled;
            set => NetworkLoad.Enabled = !value;
        }

        [ViewVariables]
        public PowerState.Load NetworkLoad { get; } = new PowerState.Load
        {
            DesiredPower = 5
        };

        public float PowerReceived => NetworkLoad.ReceivingPower;
    }
}
