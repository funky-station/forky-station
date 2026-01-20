// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2025 TemporalOroboros
// SPDX-FileCopyrightText: 2025 J
// SPDX-FileCopyrightText: 2024 Winkarst
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Ben
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2022 themias
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-License-Identifier: MIT

using Content.Server.Engineering.Components;
using Content.Server.Stack;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Stacks;
using JetBrains.Annotations;
using Robust.Shared.Map.Components;

namespace Content.Server.Engineering.EntitySystems
{
    [UsedImplicitly]
    public sealed class SpawnAfterInteractSystem : EntitySystem
    {
        [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
        [Dependency] private readonly StackSystem _stackSystem = default!;
        [Dependency] private readonly TurfSystem _turfSystem = default!;
        [Dependency] private readonly SharedTransformSystem _transform = default!;
        [Dependency] private readonly SharedMapSystem _maps = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<SpawnAfterInteractComponent, AfterInteractEvent>(HandleAfterInteract);
        }

        private async void HandleAfterInteract(EntityUid uid, SpawnAfterInteractComponent component, AfterInteractEvent args)
        {
            if (!args.CanReach && !component.IgnoreDistance)
                return;
            if (string.IsNullOrEmpty(component.Prototype))
                return;

            var gridUid = _transform.GetGrid(args.ClickLocation);
            if (!TryComp<MapGridComponent>(gridUid, out var grid))
                return;
            if (!_maps.TryGetTileRef(gridUid.Value, grid, args.ClickLocation, out var tileRef))
                return;

            bool IsTileClear()
            {
                return tileRef.Tile.IsEmpty == false && !_turfSystem.IsTileBlocked(tileRef, CollisionGroup.MobMask);
            }

            if (!IsTileClear())
                return;

            if (component.DoAfterTime > 0)
            {
                var doAfterArgs = new DoAfterArgs(EntityManager, args.User, component.DoAfterTime, new AwaitedDoAfterEvent(), null)
                {
                    BreakOnMove = true,
                };
                var result = await _doAfterSystem.WaitDoAfter(doAfterArgs);

                if (result != DoAfterStatus.Finished)
                    return;
            }

            if (component.Deleted || !IsTileClear())
                return;

            if (TryComp<StackComponent>(uid, out var stackComp)
                && component.RemoveOnInteract && !_stackSystem.TryUse((uid, stackComp), 1))
            {
                return;
            }

            Spawn(component.Prototype, args.ClickLocation.SnapToGrid(grid));

            if (component.RemoveOnInteract && stackComp == null)
                TryQueueDel(uid);
        }
    }
}
