// SPDX-FileCopyrightText: 2025 kosticia
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2024 Cojoke
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2023 08A
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2023 gus
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Shared.IgnitionSource;

namespace Content.Server.IgnitionSource;

public sealed partial class IgnitionSourceSystem : SharedIgnitionSourceSystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<IgnitionSourceComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            if (!comp.Ignited)
                continue;

            if (xform.GridUid is { } gridUid)
            {
                var position = _transform.GetGridOrMapTilePosition(uid, xform);
                // TODO: Should this be happening every single tick?
                _atmosphere.HotspotExpose(gridUid, position, comp.Temperature, 50, uid, true);
            }
        }
    }
}
