// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024-2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.DeviceNetwork.Components;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.DeviceNetwork.Events;

namespace Content.Server.DeviceNetwork.Systems;

public sealed class DeviceNetworkRequiresPowerSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<DeviceNetworkRequiresPowerComponent, BeforePacketSentEvent>(OnBeforePacketSent);
    }

    private void OnBeforePacketSent(EntityUid uid, DeviceNetworkRequiresPowerComponent component,
        BeforePacketSentEvent args)
    {
        if (!this.IsPowered(uid, EntityManager))
        {
            args.Cancel();
        }
    }
}
