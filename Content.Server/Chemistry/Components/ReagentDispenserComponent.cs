// SPDX-FileCopyrightText: 2025 pathetic meowmeow
// SPDX-FileCopyrightText: 2022 0x6273
// SPDX-FileCopyrightText: 2024 0x6273
// SPDX-FileCopyrightText: 2024 Kevin Zheng
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 Júlio César Ueti
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 moonheart08
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2021 py01
// SPDX-FileCopyrightText: 2020 derek
// SPDX-FileCopyrightText: 2020 DTanxxx
// SPDX-FileCopyrightText: 2020 nuke
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 AJCM-git
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 Memory
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-FileCopyrightText: 2020 Clyybber
// SPDX-FileCopyrightText: 2020 Injazz
// SPDX-FileCopyrightText: 2020 PrPleGoo
// SPDX-FileCopyrightText: 2020 JiimBob
// SPDX-FileCopyrightText: 2019 moneyl
// SPDX-FileCopyrightText: 2020 moneyl
// SPDX-License-Identifier: MIT

using Content.Shared.Whitelist;
using Content.Shared.Containers.ItemSlots;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Chemistry.Components
{
    /// <summary>
    /// A machine that dispenses reagents into a solution container from containers in its storage slots.
    /// </summary>
    [RegisterComponent]
    [Access(typeof(ReagentDispenserSystem))]
    public sealed partial class ReagentDispenserComponent : Component
    {
        [DataField]
        public ItemSlot BeakerSlot = new();

        [DataField("clickSound"), ViewVariables(VVAccess.ReadWrite)]
        public SoundSpecifier ClickSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg");

        [ViewVariables(VVAccess.ReadWrite)]
        public ReagentDispenserDispenseAmount DispenseAmount = ReagentDispenserDispenseAmount.U10;
    }
}
