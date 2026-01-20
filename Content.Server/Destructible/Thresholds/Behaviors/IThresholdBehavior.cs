// SPDX-FileCopyrightText: 2025 SlamBamActionman
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Shared.Database;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
    public interface IThresholdBehavior
    {
        public LogImpact Impact => LogImpact.Low;

        /// <summary>
        ///     Executes this behavior.
        /// </summary>
        /// <param name="owner">The entity that owns this behavior.</param>
        /// <param name="system">
        ///     An instance of <see cref="DestructibleSystem"/> to pull dependencies
        ///     and other systems from.
        /// </param>
        /// <param name="cause">The entity that caused this behavior.</param>
        void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null);
    }
}
