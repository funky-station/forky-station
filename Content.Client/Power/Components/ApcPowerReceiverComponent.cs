// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 ShadowCommander
// SPDX-License-Identifier: MIT

using Content.Shared.Power.Components;

namespace Content.Client.Power.Components;

[RegisterComponent]
public sealed partial class ApcPowerReceiverComponent : SharedApcPowerReceiverComponent
{
    public override float Load { get; set; }
}
