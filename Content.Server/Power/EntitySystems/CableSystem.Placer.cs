// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2025 IProduceWidgets
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 MilenVolf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Logs;
using Content.Server.Power.Components;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Stacks;
using Content.Shared.Whitelist;
using Robust.Shared.Map.Components;

namespace Content.Server.Power.EntitySystems;

public sealed partial class CableSystem
{
    [Dependency] private readonly IAdminLogManager _adminLogger = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelistSystem = default!;

    private void InitializeCablePlacer()
    {
        SubscribeLocalEvent<CablePlacerComponent, AfterInteractEvent>(OnCablePlacerAfterInteract);
    }

    private void OnCablePlacerAfterInteract(Entity<CablePlacerComponent> placer, ref AfterInteractEvent args)
    {
        if (args.Handled || !args.CanReach)
            return;

        var component = placer.Comp;
        if (component.CablePrototypeId == null)
            return;

        if (!TryComp<MapGridComponent>(_transform.GetGrid(args.ClickLocation), out var grid))
            return;

        var gridUid = _transform.GetGrid(args.ClickLocation)!.Value;
        var snapPos = _map.TileIndicesFor((gridUid, grid), args.ClickLocation);
        var tileDef = (ContentTileDefinition)_tileManager[_map.GetTileRef(gridUid, grid, snapPos).Tile.TypeId];

        if ((!component.OverTile && !tileDef.IsSubFloor) || !tileDef.Sturdy)
            return;

        foreach (var anchored in _map.GetAnchoredEntities((gridUid, grid), snapPos))
        {
            if (_whitelistSystem.IsWhitelistPass(component.Blacklist, anchored))
                return;

            if (TryComp<CableComponent>(anchored, out var wire) && wire.CableType == component.BlockingCableType)
                return;
        }

        if (TryComp<StackComponent>(placer, out var stack) && !_stack.TryUse((placer.Owner, stack), 1))
            return;

        var newCable = Spawn(component.CablePrototypeId, _map.GridTileToLocal(gridUid, grid, snapPos));
        _adminLogger.Add(LogType.Construction, LogImpact.Low,
            $"{ToPrettyString(args.User):player} placed {ToPrettyString(newCable):cable} at {Transform(newCable).Coordinates}");
        args.Handled = true;
    }
}
