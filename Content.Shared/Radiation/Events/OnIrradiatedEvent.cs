// SPDX-FileCopyrightText: 2025 TheGrimbeeper
// SPDX-FileCopyrightText: 2025 Fildrance
// SPDX-FileCopyrightText: 2025 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-License-Identifier: MIT

namespace Content.Shared.Radiation.Events;

/// <summary>
///     Raised on entity when it was irradiated
///     by some radiation source.
/// </summary>
public readonly record struct OnIrradiatedEvent(float FrameTime, float RadsPerSecond, EntityUid? Origin)
{
    public readonly float FrameTime = FrameTime;

    public readonly float RadsPerSecond = RadsPerSecond;

    public readonly EntityUid? Origin = Origin;

    public float TotalRads => RadsPerSecond * FrameTime;
}
