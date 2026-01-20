// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Robust.Shared.Map;

namespace Content.Shared.Construction.Conditions
{
    public interface IConstructionCondition
    {
        ConstructionGuideEntry? GenerateGuideEntry();
        bool Condition(EntityUid user, EntityCoordinates location, Direction direction);
    }
}
