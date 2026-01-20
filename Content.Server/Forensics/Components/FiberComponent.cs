// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 ike709
// SPDX-License-Identifier: MIT

namespace Content.Server.Forensics
{
    /// <summary>
    /// This controls fibers left by gloves on items,
    /// which the forensics system uses.
    /// </summary>
    [RegisterComponent]
    public sealed partial class FiberComponent : Component
    {
        [DataField]
        public LocId FiberMaterial = "fibers-synthetic";

        [DataField]
        public string? FiberColor;
    }
}
