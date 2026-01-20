// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 Julian Giebel
// SPDX-License-Identifier: MIT

using Content.Shared.DeviceNetwork;

namespace Content.Shared.DeviceLinking.Events;

[ByRefEvent]
public readonly record struct SignalReceivedEvent(string Port, EntityUid? Trigger = null, NetworkPayload? Data = null);
