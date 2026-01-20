// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Server.Pointing.EntitySystems;
using Content.Shared.Pointing.Components;

namespace Content.Server.Pointing.Components
{
    [RegisterComponent]
    [Access(typeof(PointingSystem))]
    public sealed partial class PointingArrowComponent : SharedPointingArrowComponent
    {
        /// <summary>
        ///     Whether or not this arrow will convert into a
        ///     <see cref="RoguePointingArrowComponent"/> when its duration runs out.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("rogue")]
        public bool Rogue;
    }
}
