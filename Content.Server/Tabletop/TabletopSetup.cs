// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 eclips_e
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Tabletop
{
    [ImplicitDataDefinitionForInheritors]
    public abstract partial class TabletopSetup
    {
        /// <summary>
        ///     Method for setting up a tabletop. Use this to spawn the board and pieces, etc.
        ///     Make sure you add every entity you create to the Entities hashset in the session.
        /// </summary>
        /// <param name="session">Tabletop session to set up. You'll want to grab the tabletop center position here for spawning entities.</param>
        /// <param name="entityManager">Dependency that can be used for spawning entities.</param>
        public abstract void SetupTabletop(TabletopSession session, IEntityManager entityManager);

        [DataField("boardPrototype", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string BoardPrototype = default!;
    }
}
