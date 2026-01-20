// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2022 ike709
// SPDX-License-Identifier: MIT

namespace Content.Server.Forensics
{
    /// <summary>
    /// Used to take a sample of someone's fingerprints.
    /// </summary>
    [RegisterComponent]
    public sealed partial class ForensicPadComponent : Component
    {
        [DataField("scanDelay")]
        public float ScanDelay = 3.0f;

        public bool Used = false;
        public String Sample = string.Empty;
    }
}
