// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 Callmore
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-License-Identifier: MIT

namespace Content.Shared.CombatMode;

[ByRefEvent]
public record struct DisarmedEvent(EntityUid Target, EntityUid Source, float PushProb)
{
    /// <summary>
    /// The entity being disarmed.
    /// </summary>
    public readonly EntityUid Target = Target;

    /// <summary>
    /// The entity performing the disarm.
    /// </summary>
    public readonly EntityUid Source = Source;

    /// <summary>
    /// Probability for push/knockdown.
    /// </summary>
    public readonly float PushProbability = PushProb;

    /// <summary>
    /// Prefix for the popup message that will be displayed on a successful push.
    /// Should be set before returning.
    /// </summary>
    public string PopupPrefix = "";

    /// <summary>
    /// Whether the entity was successfully stunned from a shove.
    /// </summary>
    public bool IsStunned;

    public bool Handled;
}
