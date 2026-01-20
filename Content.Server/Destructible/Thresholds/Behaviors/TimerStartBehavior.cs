// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2024 Saphire Lattice
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-License-Identifier: MIT

namespace Content.Server.Destructible.Thresholds.Behaviors;

[DataDefinition]
public sealed partial class TimerStartBehavior : IThresholdBehavior
{
    public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
    {
        system.TriggerSystem.ActivateTimerTrigger(owner, cause);
    }
}
