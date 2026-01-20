// SPDX-FileCopyrightText: 2022 Ygg01
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Linq;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Shared.Directions;

public static class SharedDirectionExtensions
{
    public static EntityCoordinates Offset(this EntityCoordinates coordinates, Direction direction)
    {
        return coordinates.Offset(direction.ToVec());
    }
}
