// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Julian Giebel
// SPDX-FileCopyrightText: 2022 20kdc
// SPDX-License-Identifier: MIT

namespace Content.Server.DeviceLinking.Components;

/// <summary>
/// This is used for automatic linkage with various receivers, like shutters.
/// </summary>
[RegisterComponent]
public sealed partial class AutoLinkTransmitterComponent : Component
{
    [DataField("channel", required: true)]
    public string AutoLinkChannel = default!;
}

