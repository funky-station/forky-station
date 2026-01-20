// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2020 Tyler Young
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-License-Identifier: MIT

using BenchmarkDotNet.Attributes;
using Moq;
using Robust.Shared.Analyzers;
using Robust.Shared.Exceptions;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Reflection;

namespace Content.Benchmarks
{
    [Virtual]
    public partial class EntityManagerGetAllComponents
    {
        private IEntityManager _entityManager;

        [Params(5000)] public int N { get; set; }

        public static void TestRun()
        {
            var x = new EntityManagerGetAllComponents
            {
                N = 500
            };
            x.Setup();
            x.Run();
        }

        [GlobalSetup]
        public void Setup()
        {
            // Initialize component manager.
            IoCManager.InitThread();

            IoCManager.Register<IEntityManager, EntityManager>();
            IoCManager.Register<IRuntimeLog, RuntimeLog>();
            IoCManager.Register<ILogManager, LogManager>();
            IoCManager.Register<IDynamicTypeFactory, DynamicTypeFactory>();
            IoCManager.Register<IEntitySystemManager, EntitySystemManager>();
            IoCManager.RegisterInstance<IReflectionManager>(new Mock<IReflectionManager>().Object);

            var dummyReg = new ComponentRegistration(
                "Dummy",
                typeof(DummyComponent),
                CompIdx.Index<DummyComponent>());

            var componentFactory = new Mock<IComponentFactory>();
            componentFactory.Setup(p => p.GetComponent<DummyComponent>()).Returns(new DummyComponent());
            componentFactory.Setup(m => m.GetIndex(typeof(DummyComponent))).Returns(CompIdx.Index<DummyComponent>());
            componentFactory.Setup(p => p.GetRegistration(It.IsAny<DummyComponent>())).Returns(dummyReg);
            componentFactory.Setup(p => p.GetAllRegistrations()).Returns(new[] { dummyReg });
            componentFactory.Setup(p => p.GetAllRefTypes()).Returns(new[] { CompIdx.Index<DummyComponent>() });

            IoCManager.RegisterInstance<IComponentFactory>(componentFactory.Object);

            IoCManager.BuildGraph();
            _entityManager = IoCManager.Resolve<IEntityManager>();
            _entityManager.Initialize();

            // Initialize N entities with one component.
            for (var i = 0; i < N; i++)
            {
                var entity = _entityManager.SpawnEntity(null, EntityCoordinates.Invalid);
                _entityManager.AddComponent<DummyComponent>(entity);
            }
        }

        [Benchmark]
        public int Run()
        {
            var count = 0;

            foreach (var _ in _entityManager.EntityQuery<DummyComponent>(true))
            {
                count += 1;
            }

            return count;
        }

        [Benchmark]
        public int Noop()
        {
            var count = 0;

            _entityManager.TryGetComponent(default, out DummyComponent _);

            return count;
        }

        private sealed partial class DummyComponent : Component
        {
        }
    }
}
