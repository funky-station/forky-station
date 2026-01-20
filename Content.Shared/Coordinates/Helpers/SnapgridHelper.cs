// SPDX-FileCopyrightText: 2025 B_Kirill
// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Ben
// SPDX-FileCopyrightText: 2022 SpaceManiac
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Shared.Coordinates.Helpers
{
    public static class SnapgridHelper
    {
        public static EntityCoordinates SnapToGrid(this EntityCoordinates coordinates, IEntityManager? entMan = null, IMapManager? mapManager = null)
        {
            IoCManager.Resolve(ref entMan, ref mapManager);
            var xformSys = entMan.System<SharedTransformSystem>();

            var gridId = xformSys.GetGrid(coordinates.EntityId);

            if (gridId == null)
            {
                var mapPos = xformSys.ToMapCoordinates(coordinates);
                var mapX = (int)Math.Floor(mapPos.X) + 0.5f;
                var mapY = (int)Math.Floor(mapPos.Y) + 0.5f;
                mapPos = new MapCoordinates(new Vector2(mapX, mapY), mapPos.MapId);
                return xformSys.ToCoordinates(coordinates.EntityId, mapPos);
            }

            var grid = entMan.GetComponent<MapGridComponent>(gridId.Value);
            var tileSize = grid.TileSize;
            var localPos = xformSys.WithEntityId(coordinates, gridId.Value).Position;
            var x = (int)Math.Floor(localPos.X / tileSize) + tileSize / 2f;
            var y = (int)Math.Floor(localPos.Y / tileSize) + tileSize / 2f;
            var gridPos = new EntityCoordinates(gridId.Value, new Vector2(x, y));
            return xformSys.WithEntityId(gridPos, coordinates.EntityId);
        }

        public static EntityCoordinates SnapToGrid(this EntityCoordinates coordinates, MapGridComponent grid)
        {
            var tileSize = grid.TileSize;

            var localPos = coordinates.Position;

            var x = (int)Math.Floor(localPos.X / tileSize) + tileSize / 2f;
            var y = (int)Math.Floor(localPos.Y / tileSize) + tileSize / 2f;

            return new EntityCoordinates(coordinates.EntityId, x, y);
        }
    }
}
