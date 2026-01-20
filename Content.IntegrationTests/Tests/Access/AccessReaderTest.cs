// SPDX-FileCopyrightText: 2025 chromiumboy
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 c4llv07e
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Kara D
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests.Access
{
    [TestFixture]
    [TestOf(typeof(AccessReaderComponent))]
    public sealed class AccessReaderTest
    {
        [TestPrototypes]
        private const string Prototypes = @"
- type: entity
  id: TestAccessReader
  name: access reader
  components:
  - type: AccessReader
";

        [Test]
        public async Task TestTags()
        {
            await using var pair = await PoolManager.GetServerClient();
            var server = pair.Server;
            var entityManager = server.ResolveDependency<IEntityManager>();

            await server.WaitAssertion(() =>
            {
                var system = entityManager.System<AccessReaderSystem>();
                var ent = entityManager.SpawnEntity("TestAccessReader", MapCoordinates.Nullspace);
                var reader = new Entity<AccessReaderComponent>(ent, entityManager.GetComponent<AccessReaderComponent>(ent));

                // test empty
                Assert.Multiple(() =>
                {
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "Foo" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "Bar" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(Array.Empty<ProtoId<AccessLevelPrototype>>(), reader), Is.True);
                });

                // test deny
                system.AddDenyTag(reader, "A");
                Assert.Multiple(() =>
                {
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "Foo" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "A" }, reader), Is.False);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "A", "Foo" }, reader), Is.False);
                    Assert.That(system.AreAccessTagsAllowed(Array.Empty<ProtoId<AccessLevelPrototype>>(), reader), Is.True);
                });
                system.ClearDenyTags(reader);

                // test one list
                system.TryAddAccess(reader, "A");
                Assert.Multiple(() =>
                {
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "A" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "B" }, reader), Is.False);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "A", "B" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(Array.Empty<ProtoId<AccessLevelPrototype>>(), reader), Is.False);
                });
                system.TryClearAccesses(reader);

                // test one list - two items
                system.TryAddAccess(reader, new HashSet<ProtoId<AccessLevelPrototype>> { "A", "B" });
                Assert.Multiple(() =>
                {
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "A" }, reader), Is.False);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "B" }, reader), Is.False);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "A", "B" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(Array.Empty<ProtoId<AccessLevelPrototype>>(), reader), Is.False);
                });
                system.TryClearAccesses(reader);

                // test two list
                var accesses = new List<HashSet<ProtoId<AccessLevelPrototype>>>() {
                    new HashSet<ProtoId<AccessLevelPrototype>> () { "A" },
                    new HashSet<ProtoId<AccessLevelPrototype>> () { "B", "C" }
                };
                system.TryAddAccesses(reader, accesses);
                Assert.Multiple(() =>
                {
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "A" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "B" }, reader), Is.False);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "A", "B" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "C", "B" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "C", "B", "A" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(Array.Empty<ProtoId<AccessLevelPrototype>>(), reader), Is.False);
                });
                system.TryClearAccesses(reader);

                // test deny list
                system.TryAddAccess(reader, new HashSet<ProtoId<AccessLevelPrototype>> { "A" });
                system.AddDenyTag(reader, "B");
                Assert.Multiple(() =>
                {
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "A" }, reader), Is.True);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "B" }, reader), Is.False);
                    Assert.That(system.AreAccessTagsAllowed(new List<ProtoId<AccessLevelPrototype>> { "A", "B" }, reader), Is.False);
                    Assert.That(system.AreAccessTagsAllowed(Array.Empty<ProtoId<AccessLevelPrototype>>(), reader), Is.False);
                });
                system.TryClearAccesses(reader);
                system.ClearDenyTags(reader);
            });
            await pair.CleanReturnAsync();
        }

    }
}
