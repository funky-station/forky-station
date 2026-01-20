// SPDX-FileCopyrightText: 2025 IProduceWidgets
// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 moonheart08
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 DmitriyRubetskoy
// SPDX-FileCopyrightText: 2020 ColdAutumnRain
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2020 Clyybber
// SPDX-FileCopyrightText: 2020 Tad Hardesty
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Power;
using Content.Shared.Whitelist;

namespace Content.Server.Power.Components
{
    [RegisterComponent]
    public sealed partial class CablePlacerComponent : Component
    {
        /// <summary>
        /// The structure prototype for the cable coil to place.
        /// </summary>
        [DataField("cablePrototypeID", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string? CablePrototypeId = "CableHV";

        /// <summary>
        /// What kind of wire prevents placing this wire over it as CableType.
        /// </summary>
        [DataField("blockingWireType")]
        public CableType BlockingCableType = CableType.HighVoltage;

        /// <summary>
        /// Blacklist for things the cable cannot be placed over. For things that arent cables with CableTypes.
        /// </summary>
        [DataField]
        public EntityWhitelist Blacklist = new();

        /// <summary>
        /// Whether the placed cable should go over tiles or not.
        /// </summary>
        [DataField]
        public bool OverTile;
    }
}
