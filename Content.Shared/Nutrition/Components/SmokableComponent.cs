// SPDX-FileCopyrightText: 2025 deltanedas
// SPDX-FileCopyrightText: 2023 LordEclipse
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 brainfood1183
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-License-Identifier: MIT

using Content.Shared.FixedPoint;
using Content.Shared.Smoking;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Nutrition.Components
{
    [RegisterComponent, NetworkedComponent]
    public sealed partial class SmokableComponent : Component
    {
        [DataField("solution")]
        public string Solution { get; private set; } = "smokable";

        /// <summary>
        ///     Solution inhale amount per second.
        /// </summary>
        [DataField("inhaleAmount"), ViewVariables(VVAccess.ReadWrite)]
        public FixedPoint2 InhaleAmount { get; private set; } = FixedPoint2.New(0.05f);

        [DataField("state")]
        public SmokableState State { get; set; } = SmokableState.Unlit;

        [DataField("exposeTemperature"), ViewVariables(VVAccess.ReadWrite)]
        public float ExposeTemperature { get; set; } = 0;

        [DataField("exposeVolume"), ViewVariables(VVAccess.ReadWrite)]
        public float ExposeVolume { get; set; } = 1f;

        // clothing prefixes
        [DataField("burntPrefix")]
        public string BurntPrefix = "unlit";
        [DataField("litPrefix")]
        public string LitPrefix = "lit";
        [DataField("unlitPrefix")]
        public string UnlitPrefix = "unlit";

        /// <summary>
        /// Sound played when lighting this smokable.
        /// </summary>
        [DataField]
        public SoundSpecifier? LightSound = new SoundPathSpecifier("/Audio/Effects/cig_light.ogg");

        /// <summary>
        /// Sound played when this smokable is extinguished or runs out.
        /// </summary>
        [DataField]
        public SoundSpecifier? SnuffSound = new SoundPathSpecifier("/Audio/Effects/cig_snuff.ogg");
    }
}
