// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.DeviceNetwork.Components;

namespace Content.Shared.DeviceNetwork.Events;

/// <summary>
/// Sent to the sending entity before broadcasting network packets to recipients
/// </summary>
public sealed class BeforeBroadcastAttemptEvent : CancellableEntityEventArgs
{
    public readonly IReadOnlySet<DeviceNetworkComponent> Recipients;
    public HashSet<DeviceNetworkComponent>? ModifiedRecipients;

    public BeforeBroadcastAttemptEvent(IReadOnlySet<DeviceNetworkComponent> recipients)
    {
        Recipients = recipients;
    }
}
