// SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Saphire Lattice <lattice@saphi.re>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
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
