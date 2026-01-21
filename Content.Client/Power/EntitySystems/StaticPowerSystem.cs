// SPDX-FileCopyrightText: 2022-2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Client.Power.Components;

namespace Content.Client.Power.EntitySystems;

public static class StaticPowerSystem
{
    // Using this makes the call shorter.
    // ReSharper disable once UnusedParameter.Global
    public static bool IsPowered(this EntitySystem system, EntityUid uid, IEntityManager entManager, ApcPowerReceiverComponent? receiver = null)
    {
        if (receiver == null && !entManager.TryGetComponent(uid, out receiver))
            return true;

        return receiver.Powered;
    }
}
