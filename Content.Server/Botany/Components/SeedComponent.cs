// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2024 Flesh
// SPDX-FileCopyrightText: 2024 Magnus Larsen
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Botany.Systems;
using Content.Shared.Botany.Components;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Botany.Components
{
    [RegisterComponent, Access(typeof(BotanySystem))]
    public sealed partial class SeedComponent : SharedSeedComponent
    {
        /// <summary>
        ///     Seed data containing information about the plant type & properties that this seed can grow seed. If
        ///     null, will instead attempt to get data from a seed prototype, if one is defined. See <see
        ///     cref="SeedId"/>.
        /// </summary>
        [DataField("seed")]
        public SeedData? Seed;

        /// <summary>
        ///     If not null, overrides the plant's initial health. Otherwise, the plant's initial health is set to the Endurance value.
        /// </summary>
        [DataField]
        public float? HealthOverride = null;

        /// <summary>
        ///     Name of a base seed prototype that is used if <see cref="Seed"/> is null.
        /// </summary>
        [DataField("seedId", customTypeSerializer: typeof(PrototypeIdSerializer<SeedPrototype>))]
        public string? SeedId;
    }
}
