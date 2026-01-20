// SPDX-FileCopyrightText: 2025 ArtisticRoomba
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 TekuNut
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 nuke
// SPDX-FileCopyrightText: 2020 Vince
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-License-Identifier: MIT

using Robust.Shared.Map;

namespace Content.Server.Chemistry.Components
{
    [RegisterComponent]
    public sealed partial class VaporComponent : Component
    {
        public const string SolutionName = "vapor";

        /// <summary>
        /// Stores data on the previously reacted tile. We only want to do reaction checks once per tile.
        /// </summary>
        [DataField]
        public TileRef? PreviousTileRef;

        /// <summary>
        /// Percentage of the reagent that is reacted with the TileReaction.
        /// <example>
        /// 0.5 = 50% of the reagent is reacted.
        /// </example>
        /// </summary>
        [DataField]
        public float TransferAmountPercentage;

        /// <summary>
        /// The minimum amount of the reagent that will be reacted with the TileReaction.
        /// We do this to prevent floating point issues. A reagent with a low percentage transfer amount will
        /// transfer 0.01~ forever and never get deleted.
        /// <remarks>Defaults to 0.05 if not defined, a good general value.</remarks>
        /// </summary>
        [DataField]
        public float MinimumTransferAmount = 0.05f;

        [DataField]
        public bool Active;
    }
}
