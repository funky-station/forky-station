// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Power;

/// <summary>
/// Raised whenever an ApcPowerReceiver becomes powered / unpowered.
/// Does nothing on the client.
/// </summary>
[ByRefEvent]
public readonly record struct PowerChangedEvent(bool Powered, float ReceivingPower);