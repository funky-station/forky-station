// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Server.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.IntegrationTests.Tests
{
    [TestFixture]
    public sealed class ShuttleTest
    {
        [Test]
        public async Task Test()
        {
            await using var pair = await PoolManager.GetServerClient();
            var server = pair.Server;
            await server.WaitIdleAsync();

            var mapMan = server.ResolveDependency<IMapManager>();
            var entManager = server.ResolveDependency<IEntityManager>();
            var physicsSystem = entManager.System<SharedPhysicsSystem>();

            PhysicsComponent gridPhys = null;

            var map = await pair.CreateTestMap();

            await server.WaitAssertion(() =>
            {
                var mapId = map.MapId;
                var grid = map.Grid;

                Assert.Multiple(() =>
                {
                    Assert.That(entManager.HasComponent<ShuttleComponent>(grid));
                    Assert.That(entManager.TryGetComponent(grid, out gridPhys));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(gridPhys.BodyType, Is.EqualTo(BodyType.Dynamic));
                    Assert.That(entManager.GetComponent<TransformComponent>(grid).LocalPosition, Is.EqualTo(Vector2.Zero));
                });
                physicsSystem.ApplyLinearImpulse(grid, Vector2.One, body: gridPhys);
            });

            await server.WaitRunTicks(1);

            await server.WaitAssertion(() =>
            {
                Assert.That(entManager.GetComponent<TransformComponent>(map.Grid).LocalPosition, Is.Not.EqualTo(Vector2.Zero));
            });
            await pair.CleanReturnAsync();
        }
    }
}
