// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-License-Identifier: MIT

using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using JetBrains.Annotations;
using Robust.Shared.Containers;

namespace Content.Server.Storage.EntitySystems
{
    [UsedImplicitly]
    public sealed class ItemCounterSystem : SharedItemCounterSystem
    {
        [Dependency] private readonly EntityWhitelistSystem _whitelistSystem = default!;
        protected override int? GetCount(ContainerModifiedMessage msg, ItemCounterComponent itemCounter)
        {
            if (!TryComp(msg.Container.Owner, out StorageComponent? component))
            {
                return null;
            }

            var count = 0;
            foreach (var entity in component.Container.ContainedEntities)
            {
                if (_whitelistSystem.IsWhitelistPass(itemCounter.Count, entity))
                    count++;
            }

            return count;
        }
    }
}
