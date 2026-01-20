// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared.Prototypes
{
    [UsedImplicitly]
    public static class EntityPrototypeHelpers
    {
        public static bool HasComponent<T>(this EntityPrototype prototype, IComponentFactory? componentFactory = null) where T : IComponent
        {
            return prototype.HasComponent(typeof(T), componentFactory);
        }

        public static bool HasComponent(this EntityPrototype prototype, Type component, IComponentFactory? componentFactory = null)
        {
            componentFactory ??= IoCManager.Resolve<IComponentFactory>();

            var registration = componentFactory.GetRegistration(component);

            return prototype.Components.ContainsKey(registration.Name);
        }

        public static bool HasComponent<T>(string prototype, IPrototypeManager? prototypeManager = null, IComponentFactory? componentFactory = null) where T : IComponent
        {
            return HasComponent(prototype, typeof(T), prototypeManager, componentFactory);
        }

        public static bool HasComponent(string prototype, Type component, IPrototypeManager? prototypeManager = null, IComponentFactory? componentFactory = null)
        {
            prototypeManager ??= IoCManager.Resolve<IPrototypeManager>();

            return prototypeManager.TryIndex(prototype, out EntityPrototype? proto) && proto.HasComponent(component, componentFactory);
        }
    }
}
