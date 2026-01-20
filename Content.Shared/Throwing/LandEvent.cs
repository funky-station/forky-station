// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-License-Identifier: MIT

namespace Content.Shared.Throwing
{
    /// <summary>
    ///     Raised when an entity that was thrown lands. This occurs before they stop moving and is when their tile-friction is reapplied.
    /// </summary>
    [ByRefEvent]
    public readonly record struct LandEvent(EntityUid? User, bool PlaySound);

    /// <summary>
    /// Raised when a thrown entity is no longer moving.
    /// </summary>
    [ByRefEvent]
    public record struct StopThrowEvent(EntityUid? User);
}
