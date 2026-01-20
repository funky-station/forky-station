// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Wrexbe
// SPDX-License-Identifier: MIT

using Content.Server.Holiday;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Roles;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Jobs
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class GiveItemOnHolidaySpecial : JobSpecial
    {
        [DataField("holiday", customTypeSerializer:typeof(PrototypeIdSerializer<HolidayPrototype>))]
        public string Holiday { get; private set; } = string.Empty;

        [DataField("prototype", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string Prototype { get; private set; } = string.Empty;

        public override void AfterEquip(EntityUid mob)
        {
            if (string.IsNullOrEmpty(Holiday) || string.IsNullOrEmpty(Prototype))
                return;

            var sysMan = IoCManager.Resolve<IEntitySystemManager>();

            if (!sysMan.GetEntitySystem<HolidaySystem>().IsCurrentlyHoliday(Holiday))
                return;

            var entMan = IoCManager.Resolve<IEntityManager>();

            var entity = entMan.SpawnEntity(Prototype, entMan.GetComponent<TransformComponent>(mob).Coordinates);

            sysMan.GetEntitySystem<SharedHandsSystem>().PickupOrDrop(mob, entity);
        }
    }
}
