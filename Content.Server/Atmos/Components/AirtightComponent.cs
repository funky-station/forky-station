// SPDX-FileCopyrightText: 2026 TriviaSolari
// SPDX-FileCopyrightText: 2026 ArtisticRoomba
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Swept
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Atmos.Components
{
    [RegisterComponent, Access(typeof(AirtightSystem))]
    public sealed partial class AirtightComponent : Component
    {
        public (EntityUid Grid, Vector2i Tile) LastPosition { get; set; }

        /// <summary>
        /// The directions in which this entity should block airflow, relative to its own reference frame.
        /// </summary>
        [DataField("airBlockedDirection", customTypeSerializer: typeof(FlagSerializer<AtmosDirectionFlags>))]
        public int InitialAirBlockedDirection { get; set; } = (int) AtmosDirection.All;

        /// <summary>
        /// The directions in which the entity is currently blocking airflow, relative to the grid that the entity is on.
        /// I.e., this is a variant of <see cref="InitialAirBlockedDirection"/> that takes into account the entity's
        /// current rotation.
        /// </summary>
        [ViewVariables]
        public int CurrentAirBlockedDirection;

        /// <summary>
        /// Whether the airtight entity is currently blocking airflow.
        /// </summary>
        [DataField]
        public bool AirBlocked { get; set; } = true;

        /// <summary>
        /// If true, entities on this tile will attempt to draw air from surrounding tiles when they become unblocked
        /// and currently have no air. This is generally only required when <see cref="NoAirWhenFullyAirBlocked"/> is
        /// true, or if the entity is likely to occupy the same tile as another no-air airtight entity.
        /// </summary>
        [DataField]
        public bool FixVacuum { get; set; } = true;
        // I think fixvacuum exists to ensure that repeatedly closing/opening air-blocking doors doesn't end up
        // depressurizing a room. However it can also effectively be used as a means of generating gasses for free
        // TODO ATMOS Mass conservation. Make it actually push/pull air from adjacent tiles instead of destroying & creating,

        // TODO ATMOS slate for removal. Stuff doesn't use this.
        [DataField("rotateAirBlocked")]
        public bool RotateAirBlocked { get; set; } = true;

        /// <summary>
        /// If true, then the tile that this entity is on will have no air at all if all directions are blocked.
        /// </summary>
        [DataField]
        public bool NoAirWhenFullyAirBlocked { get; set; } = true;

        /// <inheritdoc cref="CurrentAirBlockedDirection"/>
        [Access(Other = AccessPermissions.ReadWriteExecute)]
        public AtmosDirection AirBlockedDirection => (AtmosDirection)CurrentAirBlockedDirection;
    }
}
