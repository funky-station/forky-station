// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Server.GameTicking;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.Reflection;

namespace Content.IntegrationTests.Tests
{
    [TestFixture]
    [TestOf(typeof(RoundRestartCleanupEvent))]
    public sealed class ResettingEntitySystemTests
    {
        public sealed class TestRoundRestartCleanupEvent : EntitySystem
        {
            public bool HasBeenReset { get; set; }

            public override void Initialize()
            {
                base.Initialize();

                SubscribeLocalEvent<RoundRestartCleanupEvent>(Reset);
            }

            public void Reset(RoundRestartCleanupEvent ev)
            {
                HasBeenReset = true;
            }
        }

        [Test]
        public async Task ResettingEntitySystemResetTest()
        {
            await using var pair = await PoolManager.GetServerClient(new PoolSettings
            {
                DummyTicker = false,
                Connected = true,
                Dirty = true
            });
            var server = pair.Server;

            var entitySystemManager = server.ResolveDependency<IEntitySystemManager>();
            var gameTicker = entitySystemManager.GetEntitySystem<GameTicker>();

            await server.WaitAssertion(() =>
            {
                Assert.That(gameTicker.RunLevel, Is.EqualTo(GameRunLevel.InRound));

                var system = entitySystemManager.GetEntitySystem<TestRoundRestartCleanupEvent>();

                system.HasBeenReset = false;

                gameTicker.RestartRound();

                Assert.That(system.HasBeenReset);
            });
            await pair.CleanReturnAsync();
        }
    }
}
