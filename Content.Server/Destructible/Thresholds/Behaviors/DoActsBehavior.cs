// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-License-Identifier: MIT

using Content.Shared.Destructible;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
    [Serializable]
    [DataDefinition]
    public sealed partial class DoActsBehavior : IThresholdBehavior
    {
        /// <summary>
        ///     What acts should be triggered upon activation.
        /// </summary>
        [DataField("acts")]
        public ThresholdActs Acts { get; set; }

        public bool HasAct(ThresholdActs act)
        {
            return (Acts & act) != 0;
        }

        public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
        {
            if (HasAct(ThresholdActs.Breakage))
            {
                system.BreakEntity(owner);
            }

            if (HasAct(ThresholdActs.Destruction))
            {
                system.DestroyEntity(owner);
            }
        }
    }
}
