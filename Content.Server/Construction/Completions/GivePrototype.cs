// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2024 ShadowCommander
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 rolfero
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Git-Nivrak
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Stack;
using Content.Shared.Construction;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Prototypes;
using Content.Shared.Stacks;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Server.Construction.Completions;

[UsedImplicitly]
[DataDefinition]
public sealed partial class GivePrototype : IGraphAction
{
    [DataField]
    public EntProtoId Prototype { get; private set; } = string.Empty;

    [DataField]
    public int Amount { get; private set; } = 1;

    public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
    {
        if (string.IsNullOrEmpty(Prototype))
            return;

        if (EntityPrototypeHelpers.HasComponent<StackComponent>(Prototype))
        {
            var stackSystem = entityManager.EntitySysManager.GetEntitySystem<StackSystem>();
            var stacks = stackSystem.SpawnMultipleNextToOrDrop(Prototype, Amount, userUid ?? uid);

            if (userUid is null || !entityManager.TryGetComponent(userUid, out HandsComponent? handsComp))
                return;

            foreach (var item in stacks)
            {
                stackSystem.TryMergeToHands(item, (userUid.Value, handsComp));
            }
        }
        else
        {
            var handsSystem = entityManager.EntitySysManager.GetEntitySystem<SharedHandsSystem>();
            var handsComp = userUid is not null ? entityManager.GetComponent<HandsComponent>(userUid.Value) : null;
            for (var i = 0; i < Amount; i++)
            {
                var item = entityManager.SpawnNextToOrDrop(Prototype, userUid ?? uid);
                handsSystem.PickupOrDrop(userUid, item, handsComp: handsComp);
            }
        }
    }
}
