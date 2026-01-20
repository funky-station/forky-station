// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Moony
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Jezithyr
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 DamianX
// SPDX-License-Identifier: MIT

using Content.Shared.Administration.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests.Commands
{
    [TestFixture]
    [TestOf(typeof(RejuvenateSystem))]
    public sealed class RejuvenateTest
    {
        private static readonly ProtoId<DamageGroupPrototype> TestDamageGroup = "Toxin";

        [TestPrototypes]
        private const string Prototypes = @"
- type: entity
  name: DamageableDummy
  id: DamageableDummy
  components:
  - type: Damageable
    damageContainer: Biological
  - type: MobState
  - type: MobThresholds
    thresholds:
      0: Alive
      200: Dead
";

        [Test]
        public async Task RejuvenateDeadTest()
        {
            await using var pair = await PoolManager.GetServerClient();
            var server = pair.Server;
            var entManager = server.ResolveDependency<IEntityManager>();
            var prototypeManager = server.ResolveDependency<IPrototypeManager>();
            var mobStateSystem = entManager.System<MobStateSystem>();
            var damSystem = entManager.System<DamageableSystem>();
            var rejuvenateSystem = entManager.System<RejuvenateSystem>();

            await server.WaitAssertion(() =>
            {
                var human = entManager.SpawnEntity("DamageableDummy", MapCoordinates.Nullspace);
                DamageableComponent damageable = null;
                MobStateComponent mobState = null;

                // Sanity check
                Assert.Multiple(() =>
                {
                    Assert.That(entManager.TryGetComponent(human, out damageable));
                    Assert.That(entManager.TryGetComponent(human, out mobState));
                });
                Assert.Multiple(() =>
                {
                    Assert.That(mobStateSystem.IsAlive(human, mobState), Is.True);
                    Assert.That(mobStateSystem.IsCritical(human, mobState), Is.False);
                    Assert.That(mobStateSystem.IsDead(human, mobState), Is.False);
                    Assert.That(mobStateSystem.IsIncapacitated(human, mobState), Is.False);
                });

                // Kill the entity
                DamageSpecifier damage = new(prototypeManager.Index(TestDamageGroup), FixedPoint2.New(10000000));

                damSystem.TryChangeDamage(human, damage, true);

                // Check that it is dead
                Assert.Multiple(() =>
                {
                    Assert.That(mobStateSystem.IsAlive(human, mobState), Is.False);
                    Assert.That(mobStateSystem.IsCritical(human, mobState), Is.False);
                    Assert.That(mobStateSystem.IsDead(human, mobState), Is.True);
                    Assert.That(mobStateSystem.IsIncapacitated(human, mobState), Is.True);
                });

                // Rejuvenate them
                rejuvenateSystem.PerformRejuvenate(human);

                // Check that it is alive and with no damage
                Assert.Multiple(() =>
                {
                    Assert.That(mobStateSystem.IsAlive(human, mobState), Is.True);
                    Assert.That(mobStateSystem.IsCritical(human, mobState), Is.False);
                    Assert.That(mobStateSystem.IsDead(human, mobState), Is.False);
                    Assert.That(mobStateSystem.IsIncapacitated(human, mobState), Is.False);

                    Assert.That(damageable.TotalDamage, Is.EqualTo(FixedPoint2.Zero));
                });
            });
            await pair.CleanReturnAsync();
        }
    }
}
