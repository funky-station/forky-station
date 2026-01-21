// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Radio.Components;

/// <summary>
///     This component is used to tag players that are currently wearing an ACTIVE headset.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class WearingHeadsetComponent : Component
{
    [DataField]
    public EntityUid Headset;
}
