// SPDX-FileCopyrightText: 2020-2021, 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020-2021 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2020-2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2020-2021 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2020-2021 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2020-2021 py01 <60152240+collinlunn@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Exp <theexp111@gmail.com>
// SPDX-FileCopyrightText: 2020 Kmc2000 <bluekorben2000@yahoo.com>
// SPDX-FileCopyrightText: 2021-2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 E F R <602406+Efruit@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Galactic Chimp <63882831+GalacticChimp@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kevin Zheng <kevinz5000@gmail.com>
// SPDX-FileCopyrightText: 2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 chromiumboy <50505512+chromiumboy@users.noreply.github.com>
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
