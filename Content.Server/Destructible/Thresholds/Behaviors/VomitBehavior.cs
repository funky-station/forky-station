// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2025 Winkarst-cpu
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
