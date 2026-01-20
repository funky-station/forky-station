// SPDX-FileCopyrightText: 2025 IProduceWidgets
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Damage;

namespace Content.Server.Damage.Components
{
    [RegisterComponent]
    public sealed partial class DamageOnLandComponent : Component
    {
        /// <summary>
        /// Should this entity be damaged when it lands regardless of its resistances?
        /// </summary>
        [DataField("ignoreResistances")]
        [ViewVariables(VVAccess.ReadWrite)]
        public bool IgnoreResistances = false;

        /// <summary>
        /// How much damage.
        /// </summary>
        [DataField("damage", required: true)]
        [ViewVariables(VVAccess.ReadWrite)]
        public DamageSpecifier Damage = default!;
    }
}
