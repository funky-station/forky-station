// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Construction;
using JetBrains.Annotations;
using Robust.Server.GameObjects;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization;

namespace Content.Server.Construction.Completions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class VisualizerDataInt : IGraphAction
    {
        [DataField("key")] public string Key { get; private set; } = string.Empty;
        [DataField("data")] public int Data { get; private set; } = 0;

        public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
        {
            if (string.IsNullOrEmpty(Key))
                return;

            if (entityManager.TryGetComponent(uid, out AppearanceComponent? appearance))
            {
                if (IoCManager.Resolve<IReflectionManager>().TryParseEnumReference(Key, out var @enum))
                {
                    entityManager.System<AppearanceSystem>().SetData(uid, @enum, Data, appearance);
                }
            }
        }
    }
}
