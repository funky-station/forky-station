// SPDX-FileCopyrightText: 2025 J
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 TemporalOroboros
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Ben
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 tmtmtl30
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Coordinates.Helpers;
using Content.Shared.Construction;
using JetBrains.Annotations;

namespace Content.Server.Construction.Completions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class SnapToGrid : IGraphAction
    {
        [DataField("southRotation")] public bool SouthRotation { get; private set; }

        public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
        {
            var transform = entityManager.GetComponent<TransformComponent>(uid);

            if (!transform.Anchored)
                entityManager.System<SharedTransformSystem>().SetCoordinates(uid, transform.Coordinates.SnapToGrid(entityManager));

            if (SouthRotation)
            {
                transform.LocalRotation = Angle.Zero;
            }
        }
    }
}
