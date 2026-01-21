// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Winkarst-cpu <74284083+Winkarst-cpu@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Medical;

namespace Content.Server.Destructible.Thresholds.Behaviors;

[DataDefinition]
public sealed partial class VomitBehavior : IThresholdBehavior
{
    public void Execute(EntityUid uid, DestructibleSystem system, EntityUid? cause = null)
    {
        system.EntityManager.System<VomitSystem>().Vomit(uid);
    }
}
