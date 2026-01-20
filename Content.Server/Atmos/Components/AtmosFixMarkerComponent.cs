// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Server.Atmos.Components
{
    /// <summary>
    /// Used by FixGridAtmos. Entities with this may get magically auto-deleted on map initialization in future.
    /// </summary>
    [RegisterComponent, EntityCategory("Mapping")]
    public sealed partial class AtmosFixMarkerComponent : Component
    {
        // See FixGridAtmos for more details
        [DataField("mode")]
        public int Mode { get; set; } = 0;
    }
}
