// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Inventory;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.IntegrationTests.Tests
{
    [TestFixture]
    public sealed class DeleteInventoryTest
    {
        // Test that when deleting an entity with an InventoryComponent,
        // any equipped items also get deleted.
        [Test]
        public async Task Test()
        {
            await using var pair = await PoolManager.GetServerClient();
            var server = pair.Server;
            var testMap = await pair.CreateTestMap();
            var entMgr = server.ResolveDependency<IEntityManager>();
            var sysManager = server.ResolveDependency<IEntitySystemManager>();
            var coordinates = testMap.GridCoords;

            await server.WaitAssertion(() =>
            {
                // Spawn everything.
                var invSystem = sysManager.GetEntitySystem<InventorySystem>();

                var container = entMgr.SpawnEntity(null, coordinates);
                entMgr.EnsureComponent<InventoryComponent>(container);
                entMgr.EnsureComponent<ContainerManagerComponent>(container);

                var child = entMgr.SpawnEntity(null, coordinates);
                var item = entMgr.EnsureComponent<ClothingComponent>(child);

                sysManager.GetEntitySystem<ClothingSystem>().SetSlots(child, SlotFlags.HEAD, item);

                // Equip item.
                Assert.That(invSystem.TryEquip(container, child, "head"), Is.True);

                // Delete parent.
                entMgr.DeleteEntity(container);

                // Assert that child item was also deleted.
                Assert.That(item.Deleted, Is.True);
            });
            await pair.CleanReturnAsync();
        }
    }
}
