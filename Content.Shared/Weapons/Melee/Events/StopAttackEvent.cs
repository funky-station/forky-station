// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events;

[Serializable, NetSerializable]
public sealed class StopAttackEvent : EntityEventArgs
{
    public readonly NetEntity Weapon;

    public StopAttackEvent(NetEntity weapon)
    {
        Weapon = weapon;
    }
}
