// SPDX-FileCopyrightText: 2024 PopGamer46
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 Kot
// SPDX-FileCopyrightText: 2023 nikthechampiongr
// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2023 Tom Leys
// SPDX-FileCopyrightText: 2023 Kevin Zheng
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 Theomund
// SPDX-FileCopyrightText: 2022 0x6273
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 TekuNut
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 Fishfish458
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 WlarusFromDaSpace
// SPDX-FileCopyrightText: 2022 ElectroJr
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 tmtmtl30
// SPDX-License-Identifier: MIT

using Content.Server.Access;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Power;
using Robust.Shared.Physics.Components;

namespace Content.Server.Doors.Systems;

public sealed class DoorSystem : SharedDoorSystem
{
    [Dependency] private readonly AirtightSystem _airtightSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DoorBoltComponent, PowerChangedEvent>(OnBoltPowerChanged);
    }

    protected override void SetCollidable(
        EntityUid uid,
        bool collidable,
        DoorComponent? door = null,
        PhysicsComponent? physics = null,
        OccluderComponent? occluder = null)
    {
        if (!Resolve(uid, ref door))
            return;

        if (door.ChangeAirtight && TryComp(uid, out AirtightComponent? airtight))
            _airtightSystem.SetAirblocked((uid, airtight), collidable);

        // Pathfinding / AI stuff.
        RaiseLocalEvent(new AccessReaderChangeEvent(uid, collidable));

        base.SetCollidable(uid, collidable, door, physics, occluder);
    }

    private void OnBoltPowerChanged(Entity<DoorBoltComponent> ent, ref PowerChangedEvent args)
    {
        if (args.Powered)
        {
            if (ent.Comp.BoltWireCut)
                SetBoltsDown(ent, true);
        }

        ent.Comp.Powered = args.Powered;
        Dirty(ent, ent.Comp);
        UpdateBoltLightStatus(ent);
    }
}
