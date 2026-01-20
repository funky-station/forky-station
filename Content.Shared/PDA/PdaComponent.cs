// SPDX-FileCopyrightText: 2025 SlamBamActionman
// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-FileCopyrightText: 2024 VMSolidus
// SPDX-FileCopyrightText: 2023 LordEclipse
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 0x6273
// SPDX-FileCopyrightText: 2023 Daniil Sikinami
// SPDX-FileCopyrightText: 2020 AJCM-git
// SPDX-FileCopyrightText: 2023 AJCM-git
// SPDX-FileCopyrightText: 2022 ike709
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Swept
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 20kdc
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Peter Wedder
// SPDX-FileCopyrightText: 2020 derek
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 F77F
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.PDA
{
    [RegisterComponent, NetworkedComponent]
    public sealed partial class PdaComponent : Component
    {
        public const string PdaIdSlotId = "PDA-id";
        public const string PdaPenSlotId = "PDA-pen";
        public const string PdaPaiSlotId = "PDA-pai";

        [DataField("idSlot")]
        public ItemSlot IdSlot = new();

        [DataField("penSlot")]
        public ItemSlot PenSlot = new();
        [DataField("paiSlot")]
        public ItemSlot PaiSlot = new();

        // Really this should just be using ItemSlot.StartingItem. However, seeing as we have so many different starting
        // PDA's and no nice way to inherit the other fields from the ItemSlot data definition, this makes the yaml much
        // nicer to read.
        [DataField("id", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string? IdCard;

        [ViewVariables] public EntityUid? ContainedId;
        [ViewVariables] public bool FlashlightOn;

        [ViewVariables(VVAccess.ReadWrite)] public string? OwnerName;
        // The Entity that "owns" the PDA, usually a player's character.
        // This is useful when we are doing stuff like renaming a player and want to find their PDA to change the name
        // as well.
        [ViewVariables(VVAccess.ReadWrite)] public EntityUid? PdaOwner;
        [ViewVariables] public string? StationName;
        [ViewVariables] public string? StationAlertLevel;
        [ViewVariables] public Color StationAlertColor = Color.White;
    }
}
