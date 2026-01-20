// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2021 py01
// SPDX-License-Identifier: MIT

using Content.Shared.NodeContainer;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.NodeContainer.Nodes
{
    /// <summary>
    ///     A <see cref="Node"/> that can reach other <see cref="AdjacentNode"/>s that are directly adjacent to it.
    /// </summary>
    [DataDefinition]
    public sealed partial class AdjacentNode : Node
    {
        public override IEnumerable<Node> GetReachableNodes(TransformComponent xform,
            EntityQuery<NodeContainerComponent> nodeQuery,
            EntityQuery<TransformComponent> xformQuery,
            MapGridComponent? grid,
            IEntityManager entMan)
        {
            if (!xform.Anchored || grid == null)
                yield break;

            var gridIndex = grid.TileIndicesFor(xform.Coordinates);

            foreach (var (_, node) in NodeHelpers.GetCardinalNeighborNodes(nodeQuery, grid, gridIndex))
            {
                if (node != this)
                    yield return node;
            }
        }
    }
}
