// SPDX-FileCopyrightText: 2018-2019 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2019 Silver <Silvertorch5@gmail.com>
// SPDX-FileCopyrightText: 2020-2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020-2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Exp <theexp111@gmail.com>
// SPDX-FileCopyrightText: 2020 Clyybber <darkmine956@gmail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022-2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023, 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Shared.Construction.Components;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using static Content.Shared.Interaction.SharedInteractionSystem;

namespace Content.Shared.Construction
{
    public abstract class SharedConstructionSystem : EntitySystem
    {
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] private readonly SharedMapSystem _map = default!;
        [Dependency] protected readonly IPrototypeManager PrototypeManager = default!;
        [Dependency] protected readonly SharedTransformSystem TransformSystem = default!;

        /// <summary>
        ///     Get predicate for construction obstruction checks.
        /// </summary>
        public Ignored? GetPredicate(bool canBuildInImpassable, MapCoordinates coords)
        {
            if (!canBuildInImpassable)
                return null;

            if (!_mapManager.TryFindGridAt(coords, out var gridUid, out var grid))
                return null;

            var ignored = _map.GetAnchoredEntities((gridUid, grid), coords).ToHashSet();
            return e => ignored.Contains(e);
        }

        public string GetExamineName(GenericPartInfo info)
        {
            if (info.ExamineName is not null)
                return Loc.GetString(info.ExamineName.Value);

            return PrototypeManager.Index(info.DefaultPrototype).Name;
        }
    }
}
