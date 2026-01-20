// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

#nullable enable
using System.Linq;
using Robust.Client.GameObjects;
using Robust.Client.ResourceManagement;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests
{
    [TestFixture]
    public sealed class DummyIconTest
    {
        [Test]
        public async Task Test()
        {
            await using var pair = await PoolManager.GetServerClient(new PoolSettings { Connected = true });
            var client = pair.Client;
            var prototypeManager = client.ResolveDependency<IPrototypeManager>();
            var resourceCache = client.ResolveDependency<IResourceCache>();
            var spriteSys = client.System<SpriteSystem>();

            await client.WaitAssertion(() =>
            {
                foreach (var proto in prototypeManager.EnumeratePrototypes<EntityPrototype>())
                {
                    if (proto.HideSpawnMenu || proto.Abstract || pair.IsTestPrototype(proto) || !proto.Components.ContainsKey("Sprite"))
                        continue;

                    Assert.DoesNotThrow(() =>
                    {
                        var _ = spriteSys.GetPrototypeTextures(proto).ToList();
                    }, "Prototype {0} threw an exception when getting its textures.",
                        proto.ID);
                }
            });
            await pair.CleanReturnAsync();
        }
    }
}
