// SPDX-FileCopyrightText: 2024 IgorAnt028
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 ColdAutumnRain
// SPDX-FileCopyrightText: 2020 ike709
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Server.Spawners.Components
{
    [RegisterComponent, EntityCategory("Spawner")]
    [Virtual]
    public partial class ConditionalSpawnerComponent : Component
    {
        /// <summary>
        /// A list of entities, one of which can spawn in after calling Spawn()
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public List<EntProtoId> Prototypes { get; set; } = new();

        /// <summary>
        /// A list of game rules.
        /// If at least one of them was launched in the game,
        /// an attempt will occur to spawn one of the objects in the Prototypes list
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public List<EntProtoId> GameRules = new();

        /// <summary>
        /// Chance of spawning an entity
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public float Chance { get; set; } = 1.0f;
    }
}
