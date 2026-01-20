// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2025 Nemanja
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 DamianX
// SPDX-FileCopyrightText: 2020 nuke
// SPDX-License-Identifier: MIT

#nullable enable
using Content.Server.Cuffs;
using Content.Shared.Cuffs.Components;
using Content.Shared.Hands.Components;
using Robust.Server.Console;
using Robust.Shared.GameObjects;

namespace Content.IntegrationTests.Tests.GameObjects.Components.ActionBlocking
{
    [TestFixture]
    [TestOf(typeof(CuffableComponent))]
    [TestOf(typeof(HandcuffComponent))]
    public sealed class HandCuffTest
    {
        [TestPrototypes]
        private const string Prototypes = @"
- type: entity
  name: HumanHandcuffDummy
  id: HumanHandcuffDummy
  components:
  - type: Cuffable
  - type: Hands
    hands:
      hand_right:
        location: Right
      hand_left:
        location: Left
    sortedHands:
    - hand_right
    - hand_left
  - type: ComplexInteraction

- type: entity
  name: HandcuffsDummy
  id: HandcuffsDummy
  components:
  - type: Handcuff
";

        [Test]
        public async Task Test()
        {
            await using var pair = await PoolManager.GetServerClient();
            var server = pair.Server;

            EntityUid human;
            EntityUid otherHuman;
            EntityUid cuffs;
            EntityUid secondCuffs;
            CuffableComponent cuffed = default!;
            HandsComponent hands = default!;

            var entityManager = server.ResolveDependency<IEntityManager>();
            var host = server.ResolveDependency<IServerConsoleHost>();

            var map = await pair.CreateTestMap();

            await server.WaitAssertion(() =>
            {
                var coordinates = map.MapCoords;

                var cuffableSys = entityManager.System<CuffableSystem>();
                var xformSys = entityManager.System<SharedTransformSystem>();

                // Spawn the entities
                human = entityManager.SpawnEntity("HumanHandcuffDummy", coordinates);
                otherHuman = entityManager.SpawnEntity("HumanHandcuffDummy", coordinates);
                cuffs = entityManager.SpawnEntity("HandcuffsDummy", coordinates);
                secondCuffs = entityManager.SpawnEntity("HandcuffsDummy", coordinates);

                var coords = xformSys.GetWorldPosition(otherHuman);
                xformSys.SetWorldPosition(human, coords);

                // Test for components existing
                Assert.Multiple(() =>
                {
                    Assert.That(entityManager.TryGetComponent(human, out cuffed!), $"Human has no {nameof(CuffableComponent)}");
                    Assert.That(entityManager.TryGetComponent(human, out hands!), $"Human has no {nameof(HandsComponent)}");
                    Assert.That(entityManager.TryGetComponent(cuffs, out HandcuffComponent? _), $"Handcuff has no {nameof(HandcuffComponent)}");
                    Assert.That(entityManager.TryGetComponent(secondCuffs, out HandcuffComponent? _), $"Second handcuffs has no {nameof(HandcuffComponent)}");
                });

                // Test to ensure cuffed players register the handcuffs
                cuffableSys.TryAddNewCuffs(human, human, cuffs, cuffed);
                Assert.That(cuffed.CuffedHandCount, Is.GreaterThan(0), "Handcuffing a player did not result in their hands being cuffed");

                // Test to ensure a player with 4 hands will still only have 2 hands cuffed
                AddHand(entityManager.GetNetEntity(human), host);
                AddHand(entityManager.GetNetEntity(human), host);

                Assert.Multiple(() =>
                {
                    Assert.That(cuffed.CuffedHandCount, Is.EqualTo(2));
                    Assert.That(hands.SortedHands, Has.Count.EqualTo(4));
                });

                // Test to give a player with 4 hands 2 sets of cuffs
                cuffableSys.TryAddNewCuffs(human, human, secondCuffs, cuffed);
                Assert.That(cuffed.CuffedHandCount, Is.EqualTo(4), "Player doesn't have correct amount of hands cuffed");
            });

            await pair.CleanReturnAsync();
        }

        private static void AddHand(NetEntity to, IServerConsoleHost host)
        {
            host.ExecuteCommand(null, $"addhand {to}");
        }
    }
}
