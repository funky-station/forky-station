// SPDX-FileCopyrightText: 2025 OnyxTheBrave
// SPDX-FileCopyrightText: 2025 Dora
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Illiux
// SPDX-FileCopyrightText: 2022 0x6273
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 Kevin Zheng
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Myctai
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 Fishfish458
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Spartak
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2021 py01
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 derek
// SPDX-FileCopyrightText: 2020 DTanxxx
// SPDX-FileCopyrightText: 2020 nuke
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 ike709
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

using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry;
using Robust.Shared.Audio;

namespace Content.Server.Chemistry.Components
{
    /// <summary>
    /// An industrial grade chemical manipulator with pill and bottle production included.
    /// <seealso cref="ChemMasterSystem"/>
    /// </summary>
    [RegisterComponent]
    [Access(typeof(ChemMasterSystem))]
    public sealed partial class ChemMasterComponent : Component
    {
        [DataField("pillType"), ViewVariables(VVAccess.ReadWrite)]
        public uint PillType = 0;

        [DataField("mode"), ViewVariables(VVAccess.ReadWrite)]
        public ChemMasterMode Mode = ChemMasterMode.Transfer;

        [DataField]
        public ChemMasterSortingType SortingType = ChemMasterSortingType.None;

        [DataField("pillDosageLimit", required: true), ViewVariables(VVAccess.ReadWrite)]
        public uint PillDosageLimit;

        [DataField("clickSound"), ViewVariables(VVAccess.ReadWrite)]
        public SoundSpecifier ClickSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg");

        /// <summary>
        /// Which source the chem master should draw from when making pills/bottles.
        /// </summary>
        [DataField]
        public ChemMasterDrawSource DrawSource = ChemMasterDrawSource.Internal;
    }
}
