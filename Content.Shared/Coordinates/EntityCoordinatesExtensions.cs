// SPDX-FileCopyrightText: 2024 Jake Huxell
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using System.Numerics;
using Robust.Shared.Map;

namespace Content.Shared.Coordinates
{
    public static class EntityCoordinatesExtensions
    {
        public static EntityCoordinates ToCoordinates(this EntityUid id)
        {
            return new EntityCoordinates(id, new Vector2(0, 0));
        }

        public static EntityCoordinates ToCoordinates(this EntityUid id, Vector2 offset)
        {
            return new EntityCoordinates(id, offset);
        }

        public static EntityCoordinates ToCoordinates(this EntityUid id, float x, float y)
        {
            return new EntityCoordinates(id, x, y);
        }
    }
}
