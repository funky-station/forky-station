// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Willhelm53
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Maps;
using JetBrains.Annotations;
using Robust.Shared.Map;

namespace Content.Shared.Construction.Conditions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class EmptyOrWindowValidInTile : IConstructionCondition
    {
        [DataField("tileNotBlocked")]
        private TileNotBlocked _tileNotBlocked = new();

        public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
        {
            var entManager = IoCManager.Resolve<IEntityManager>();
            var lookupSys = entManager.System<EntityLookupSystem>();

            var result = false;


            foreach (var entity in lookupSys.GetEntitiesIntersecting(location, LookupFlags.Approximate | LookupFlags.Static))
            {
                if (entManager.HasComponent<SharedCanBuildWindowOnTopComponent>(entity))
                    result = true;
            }

            if (!result)
                result = _tileNotBlocked.Condition(user, location, direction);

            return result;
        }

        public ConstructionGuideEntry GenerateGuideEntry()
        {
            return new ConstructionGuideEntry
            {
                Localization = "construction-guide-condition-empty-or-window-valid-in-tile"
            };
        }
    }
}
