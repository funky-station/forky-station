// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2025 Nemanja
// SPDX-FileCopyrightText: 2024 lzk
// SPDX-FileCopyrightText: 2024 0x6273
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Tomeno
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Flipp Syder
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 ColdAutumnRain
// SPDX-FileCopyrightText: 2020 creadth
// SPDX-License-Identifier: MIT

using Content.Server.Body.Systems;
using Content.Shared.Atmos;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Body.Components
{
    [RegisterComponent, Access(typeof(RespiratorSystem)), AutoGenerateComponentPause]
    public sealed partial class RespiratorComponent : Component
    {
        /// <summary>
        ///     Volume of our breath in liters
        /// </summary>
        [DataField]
        public float BreathVolume = Atmospherics.BreathVolume;

        /// <summary>
        ///     How much of the gas we inhale is metabolized? Value range is (0, 1]
        /// </summary>
        [DataField]
        public float Ratio = 1.0f;

        /// <summary>
        ///     The next time that this body will inhale or exhale.
        /// </summary>
        [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
        public TimeSpan NextUpdate;

        /// <summary>
        ///     The interval between updates. Each update is either inhale or exhale,
        ///     so a full cycle takes twice as long.
        /// </summary>
        [DataField]
        public TimeSpan UpdateInterval = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Multiplier applied to <see cref="UpdateInterval"/> for adjusting based on metabolic rate multiplier.
        /// </summary>
        [DataField]
        public float UpdateIntervalMultiplier = 1f;

        /// <summary>
        /// Adjusted update interval based off of the multiplier value.
        /// </summary>
        [ViewVariables]
        public TimeSpan AdjustedUpdateInterval => UpdateInterval * UpdateIntervalMultiplier;

        /// <summary>
        ///     Saturation level. Reduced by UpdateInterval each tick.
        ///     Can be thought of as 'how many seconds you have until you start suffocating' in this configuration.
        /// </summary>
        [DataField]
        public float Saturation = 5.0f;

        /// <summary>
        ///     At what level of saturation will you begin to suffocate?
        /// </summary>
        [DataField]
        public float SuffocationThreshold;

        [DataField]
        public float MaxSaturation = 5.0f;

        [DataField]
        public float MinSaturation = -2.0f;

        // TODO HYPEROXIA?

        [DataField(required: true)]
        [ViewVariables(VVAccess.ReadWrite)]
        public DamageSpecifier Damage = default!;

        [DataField(required: true)]
        [ViewVariables(VVAccess.ReadWrite)]
        public DamageSpecifier DamageRecovery = default!;

        [DataField]
        public TimeSpan GaspEmoteCooldown = TimeSpan.FromSeconds(8);

        [ViewVariables]
        public TimeSpan LastGaspEmoteTime;

        /// <summary>
        ///     The emote when gasps
        /// </summary>
        [DataField]
        public ProtoId<EmotePrototype> GaspEmote = "Gasp";

        /// <summary>
        ///     How many cycles in a row has the mob been under-saturated?
        /// </summary>
        [ViewVariables]
        public int SuffocationCycles = 0;

        /// <summary>
        ///     How many cycles in a row does it take for the suffocation alert to pop up?
        /// </summary>
        [ViewVariables]
        public int SuffocationCycleThreshold = 3;

        [ViewVariables]
        public RespiratorStatus Status = RespiratorStatus.Inhaling;
    }
}

public enum RespiratorStatus
{
    Inhaling,
    Exhaling
}
