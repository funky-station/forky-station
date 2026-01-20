// SPDX-FileCopyrightText: 2025 godisdeadLOL
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.NPC.Components
{
    /// Added when a medibot injects someone
    /// So they don't get injected again for at least a minute.
    [RegisterComponent, NetworkedComponent]
    public sealed partial class NPCRecentlyInjectedComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite), DataField("accumulator")]
        public float Accumulator = 0f;

        [ViewVariables(VVAccess.ReadWrite), DataField("removeTime")]
        public TimeSpan RemoveTime = TimeSpan.FromMinutes(1);
    }
}
