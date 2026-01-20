// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2025 J
// SPDX-FileCopyrightText: 2024 Tornado Tech
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 I.K
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2023 Tom Leys
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
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
using Content.Shared.Tag;
using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Construction.Conditions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class NoWindowsInTile : IConstructionCondition
    {
        private static readonly ProtoId<TagPrototype> WindowTag = "Window";

        public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
        {
            var entManager = IoCManager.Resolve<IEntityManager>();
            var sysMan = entManager.EntitySysManager;
            var tagSystem = sysMan.GetEntitySystem<TagSystem>();
            var lookupSys = sysMan.GetEntitySystem<EntityLookupSystem>();

            foreach (var entity in lookupSys.GetEntitiesIntersecting(location, LookupFlags.Static))
            {
                if (tagSystem.HasTag(entity, WindowTag))
                    return false;
            }

            return true;
        }

        public ConstructionGuideEntry GenerateGuideEntry()
        {
            return new ConstructionGuideEntry
            {
                Localization = "construction-step-condition-no-windows-in-tile"
            };
        }
    }
}
