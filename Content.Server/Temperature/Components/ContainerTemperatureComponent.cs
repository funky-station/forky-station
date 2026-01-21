// SPDX-FileCopyrightText: 2025 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

namespace Content.Server.Temperature.Components;

[RegisterComponent]
public sealed partial class ContainerTemperatureComponent : Component
{
    [DataField]
    public float? HeatDamageThreshold;

    [DataField]
    public float? ColdDamageThreshold;
}
