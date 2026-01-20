// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Ygg01
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 CrudeWax
// SPDX-License-Identifier: MIT

using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Server.Guardian
{
    /// <summary>
    /// Given to guardian users upon establishing a guardian link with the entity
    /// </summary>
    [RegisterComponent]
    public sealed partial class GuardianHostComponent : Component
    {
        /// <summary>
        /// Guardian hosted within the component
        /// </summary>
        /// <remarks>
        /// Can be null if the component is added at any time.
        /// </remarks>
        [DataField]
        public EntityUid? HostedGuardian;

        /// <summary>
        /// Container which holds the guardian
        /// </summary>
        [ViewVariables] public ContainerSlot GuardianContainer = default!;

        [DataField]
        public EntProtoId Action = "ActionToggleGuardian";

        [DataField] public EntityUid? ActionEntity;
    }
}
