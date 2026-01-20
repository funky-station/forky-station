// SPDX-FileCopyrightText: 2025 chromiumboy
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Content.Server.Destructible.Thresholds;

namespace Content.Server.Destructible
{
    /// <summary>
    ///     When attached to an <see cref="Robust.Shared.GameObjects.EntityUid"/>, allows it to take damage
    ///     and triggers thresholds when reached.
    /// </summary>
    [RegisterComponent]
    public sealed partial class DestructibleComponent : Component
    {
        /// <summary>
        /// A list of damage thresholds for the entity;
        /// includes their triggers and resultant behaviors
        /// </summary>
        [DataField]
        public List<DamageThreshold> Thresholds = new();

        /// <summary>
        /// Specifies whether the entity has passed a damage threshold that causes it to break
        /// </summary>
        [DataField]
        public bool IsBroken = false;
    }
}
