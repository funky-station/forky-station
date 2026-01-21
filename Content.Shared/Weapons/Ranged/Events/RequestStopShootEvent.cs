// SPDX-FileCopyrightText: 2022-2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 mq <113324899+mqole@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Events;

/// <summary>
/// Raised on the client to request it would like to stop shooting.
/// </summary>
[Serializable, NetSerializable]
public sealed class RequestStopShootEvent : EntityEventArgs
{
    public NetEntity Gun;
}
