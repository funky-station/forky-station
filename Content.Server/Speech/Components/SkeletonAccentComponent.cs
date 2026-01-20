// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Willhelm53
// SPDX-License-Identifier: MIT

namespace Content.Server.Speech.Components;

/// <summary>
///     Rattle me bones!
/// </summary>
[RegisterComponent]
public sealed partial class SkeletonAccentComponent : Component
{
    /// <summary>
    ///     Chance that the message will be appended with "ACK ACK!"
    /// </summary>
    [DataField("ackChance")]
    public float ackChance = 0.3f; // Funnier if it doesn't happen every single time
}
