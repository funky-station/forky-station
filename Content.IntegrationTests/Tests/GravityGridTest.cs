// SPDX-FileCopyrightText: 2025 Justin Pfeifler
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 Julian Giebel
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 DamianX
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2020 Jackson Lewis
// SPDX-License-Identifier: MIT

using Content.Server.Power.Components;
using Content.Shared.Gravity;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.IntegrationTests.Tests
{
    /// Tests the behavior of GravityGeneratorComponent,
    /// making sure that gravity is applied to the correct grids.
    [TestFixture]
    [TestOf(typeof(GravityGeneratorComponent))]
    public sealed class GravityGridTest
    {
        [TestPrototypes]
        private const string Prototypes = @"
- type: entity
  name: GridGravityGeneratorDummy
  id: GridGravityGeneratorDummy
  components:
  - type: GravityGenerator
  - type: PowerCharge
    windowTitle: gravity-generator-window-title
    idlePower: 50
    chargeRate: 1000000000 # Set this really high so it discharges in a single tick.
    activePower: 500
  - type: ApcPowerReceiver
  - type: UserInterface
";
        [Test]
        public async Task Test()
        {
            await using var pair = await PoolManager.GetServerClient();
            var server = pair.Server;

            var testMap = await pair.CreateTestMap();

            var entityMan = server.EntMan;
            var mapMan = server.MapMan;
            var mapSys = entityMan.System<SharedMapSystem>();

            EntityUid generator = default;
            Entity<MapGridComponent> grid1 = default;
            Entity<MapGridComponent> grid2 = default;

            // Create grids
            await server.WaitAssertion(() =>
            {
                var mapId = testMap.MapId;
                grid1 = mapMan.CreateGridEntity(mapId);
                grid2 = mapMan.CreateGridEntity(mapId);

                mapSys.SetTile(grid1, grid1, Vector2i.Zero, new Tile(1));
                mapSys.SetTile(grid2, grid2, Vector2i.Zero, new Tile(1));

                generator = entityMan.SpawnEntity("GridGravityGeneratorDummy", new EntityCoordinates(grid1, 0.5f, 0.5f));
                Assert.Multiple(() =>
                {
                    Assert.That(entityMan.HasComponent<GravityGeneratorComponent>(generator));
                    Assert.That(entityMan.HasComponent<ApcPowerReceiverComponent>(generator));
                });

                var powerComponent = entityMan.GetComponent<ApcPowerReceiverComponent>(generator);
                powerComponent.NeedsPower = false;
            });

            await server.WaitRunTicks(5);

            await server.WaitAssertion(() =>
            {
                var generatorComponent = entityMan.GetComponent<GravityGeneratorComponent>(generator);
                var powerComponent = entityMan.GetComponent<ApcPowerReceiverComponent>(generator);

                Assert.Multiple(() =>
                {
                    Assert.That(generatorComponent.GravityActive, Is.True);
                    Assert.That(!entityMan.GetComponent<GravityComponent>(grid1).Enabled);
                    Assert.That(entityMan.GetComponent<GravityComponent>(grid2).Enabled);
                });

                // Re-enable needs power so it turns off again.
                // Charge rate is ridiculously high so it finishes in one tick.
                powerComponent.NeedsPower = true;
            });

            await server.WaitRunTicks(5);

            await server.WaitAssertion(() =>
            {
                var generatorComponent = entityMan.GetComponent<GravityGeneratorComponent>(generator);

                Assert.Multiple(() =>
                {
                    Assert.That(generatorComponent.GravityActive, Is.False);
                    Assert.That(entityMan.GetComponent<GravityComponent>(grid2).Enabled, Is.False);
                });
            });

            await pair.CleanReturnAsync();
        }
    }
}
