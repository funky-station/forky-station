// SPDX-FileCopyrightText: 2026 pathetic meowmeow
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-License-Identifier: MIT

namespace Content.Server.Cargo.Components;

/// <summary>
/// This is used for calculating the price of mobs.
/// </summary>
[RegisterComponent]
public sealed partial class MobPriceComponent : Component
{
    /// <summary>
    /// The base price this mob should fetch.
    /// </summary>
    [DataField("price", required: true)]
    public double Price;

    /// <summary>
    /// The percentage of the actual price that should be granted should the appraised mob be dead.
    /// </summary>
    [DataField("deathPenalty")]
    public double DeathPenalty = 0.2f;
}
