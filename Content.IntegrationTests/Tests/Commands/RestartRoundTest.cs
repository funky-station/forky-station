// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2021 moonheart08
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Server.GameTicking;
using Content.Server.GameTicking.Commands;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.IntegrationTests.Tests.Commands
{
    [TestFixture]
    [TestOf(typeof(RestartRoundNowCommand))]
    public sealed class RestartRoundNowTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task RestartRoundAfterStart(bool lobbyEnabled)
        {
            await using var pair = await PoolManager.GetServerClient(new PoolSettings
            {
                DummyTicker = false,
                Dirty = true
            });
            var server = pair.Server;

            var configManager = server.ResolveDependency<IConfigurationManager>();
            var entityManager = server.ResolveDependency<IEntityManager>();
            var gameTicker = entityManager.System<GameTicker>();

            await pair.RunTicksSync(5);

            GameTick tickBeforeRestart = default;

            await server.WaitAssertion(() =>
            {
                Assert.That(configManager.GetCVar(CCVars.GameLobbyEnabled), Is.EqualTo(false));
                configManager.SetCVar(CCVars.GameLobbyEnabled, lobbyEnabled);

                Assert.That(gameTicker.RunLevel, Is.EqualTo(GameRunLevel.InRound));

                tickBeforeRestart = entityManager.CurrentTick;

                gameTicker.RestartRound();

                if (lobbyEnabled)
                {
                    Assert.That(gameTicker.RunLevel, Is.Not.EqualTo(GameRunLevel.InRound));
                }
            });

            await pair.RunTicksSync(15);

            await server.WaitAssertion(() =>
            {
                var tickAfterRestart = entityManager.CurrentTick;

                Assert.That(tickBeforeRestart, Is.LessThan(tickAfterRestart));
            });

            await pair.RunTicksSync(5);
            await pair.CleanReturnAsync();
        }
    }
}
