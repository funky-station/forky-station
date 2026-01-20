// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Tom Leys
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-License-Identifier: MIT

using Content.Server.Doors.Systems;
using Content.Server.Wires;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Wires;

namespace Content.Server.Doors;

public sealed partial class DoorBoltLightWireAction : ComponentWireAction<DoorBoltComponent>
{
    public override Color Color { get; set; } = Color.Lime;
    public override string Name { get; set; } = "wire-name-bolt-light";

    public override StatusLightState? GetLightState(Wire wire, DoorBoltComponent comp)
        => comp.BoltLightsEnabled ? StatusLightState.On : StatusLightState.Off;

    public override object StatusKey { get; } = AirlockWireStatus.BoltLightIndicator;

    public override bool Cut(EntityUid user, Wire wire, DoorBoltComponent door)
    {
        EntityManager.System<DoorSystem>().SetBoltLightsEnabled((wire.Owner, door), false);
        return true;
    }

    public override bool Mend(EntityUid user, Wire wire, DoorBoltComponent door)
    {
        EntityManager.System<DoorSystem>().SetBoltLightsEnabled((wire.Owner, door), true);
        return true;
    }

    public override void Pulse(EntityUid user, Wire wire, DoorBoltComponent door)
    {
        EntityManager.System<DoorSystem>().SetBoltLightsEnabled((wire.Owner, door), !door.BoltLightsEnabled);
    }
}
