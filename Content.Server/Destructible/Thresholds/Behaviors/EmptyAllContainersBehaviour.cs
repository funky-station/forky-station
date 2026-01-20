// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Robust.Shared.Containers;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
    /// <summary>
    ///     Drop all items from all containers
    /// </summary>
    [DataDefinition]
    public sealed partial class EmptyAllContainersBehaviour : IThresholdBehavior
    {
        public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
        {
            if (!system.EntityManager.TryGetComponent<ContainerManagerComponent>(owner, out var containerManager))
                return;

            foreach (var container in system.EntityManager.System<SharedContainerSystem>().GetAllContainers(owner, containerManager))
            {
                system.ContainerSystem.EmptyContainer(container, true, system.EntityManager.GetComponent<TransformComponent>(owner).Coordinates);
            }
        }
    }
}
