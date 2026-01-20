// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2021 Flipp Syder
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.SubFloor
{
    /// <summary>
    /// Simple component that automatically hides the sibling
    /// <see cref="SpriteComponent" /> when the tile it's on is not a sub floor
    /// (plating).
    /// </summary>
    /// <seealso cref="P:Content.Shared.Maps.ContentTileDefinition.IsSubFloor" />
    [NetworkedComponent]
    [RegisterComponent]
    [Access(typeof(SharedSubFloorHideSystem))]
    public sealed partial class SubFloorHideComponent : Component
    {
        /// <summary>
        ///     Whether the entity's current position has a "Floor-type" tile above its current position.
        /// </summary>
        [ViewVariables]
        public bool IsUnderCover { get; set; } = false;

        /// <summary>
        ///     Whether interactions with this entity should be blocked while it is under floor tiles.
        /// </summary>
        /// <remarks>
        ///     Useful for entities like vents, which are only partially hidden. Anchor attempts will still be blocked.
        /// </remarks>
        [DataField]
        public bool BlockInteractions { get; set; } = true;

        /// <summary>
        /// Whether this entity's ambience should be disabled when underneath the floor.
        /// </summary>
        /// <remarks>
        /// Useful for cables and piping, gives maint it's distinct noise.
        /// </remarks>
        [DataField]
        public bool BlockAmbience { get; set; } = true;

        /// <summary>
        ///     Sprite layer keys for the layers that are always visible, even if the entity is below a floor tile. E.g.,
        ///     the vent part of a vent is always visible, even though the piping is hidden.
        /// </summary>
        [DataField]
        public HashSet<Enum> VisibleLayers = new();

        /// <summary>
        /// This is used for storing the original draw depth of a t-ray revealed entity.
        /// e.g. when a t-ray revealed cable is drawn above a carpet.
        /// </summary>
        [DataField]
        public int? OriginalDrawDepth;
    }
}
