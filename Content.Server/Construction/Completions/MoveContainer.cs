// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Shared.Construction;
using JetBrains.Annotations;
using Robust.Server.Containers;
using Robust.Shared.Containers;

namespace Content.Server.Construction.Completions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class MoveContainer : IGraphAction
    {
        [DataField("from")] public string? FromContainer { get; private set; }
        [DataField("to")] public string? ToContainer { get; private set; }

        public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
        {
            if (string.IsNullOrEmpty(FromContainer) || string.IsNullOrEmpty(ToContainer))
                return;

            var containerSystem = entityManager.EntitySysManager.GetEntitySystem<ContainerSystem>();
            var containerManager = entityManager.EnsureComponent<ContainerManagerComponent>(uid);

            var from = containerSystem.EnsureContainer<Container>(uid, FromContainer, containerManager);
            var to = containerSystem.EnsureContainer<Container>(uid, ToContainer, containerManager);

            foreach (var contained in from.ContainedEntities.ToArray())
            {
                if (containerSystem.Remove(contained, from))
                    containerSystem.Insert(contained, to);
            }
        }
    }
}
