// SPDX-FileCopyrightText: 2024 IgorAnt028
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2021 AJCM-git
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 ColdAutumnRain
// SPDX-FileCopyrightText: 2020 ike709
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Server.Spawners.Components
{
    [RegisterComponent, EntityCategory("Spawner")]
    public sealed partial class RandomSpawnerComponent : ConditionalSpawnerComponent
    {
        /// <summary>
        /// A list of rarer entities that can spawn with the RareChance
        /// instead of one of the entities in the Prototypes list.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public List<EntProtoId> RarePrototypes { get; set; } = new();

        /// <summary>
        /// The chance that a rare prototype may spawn instead of a common prototype
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public float RareChance { get; set; } = 0.05f;

        /// <summary>
        /// Scatter of entity spawn coordinates
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public float Offset { get; set; } = 0.2f;

        /// <summary>
        /// A variable meaning whether the spawn will
        /// be able to be used again or whether
        /// it will be destroyed after the first use
        /// </summary>
        [DataField]
        public bool DeleteSpawnerAfterSpawn = true;
    }
}
