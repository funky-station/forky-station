// SPDX-FileCopyrightText: 2024 SlamBamActionman
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Alert;

/// <summary>
///     Handles the icons on the right side of the screen.
///     Should only be used for player-controlled entities.
/// </summary>
// Component is not AutoNetworked due to supporting clientside-only alerts.
// Component state is handled manually to avoid the server overwriting the client list.
[RegisterComponent, NetworkedComponent]
public sealed partial class AlertsComponent : Component
{
    [ViewVariables]
    public Dictionary<AlertKey, AlertState> Alerts = new();

    public override bool SendOnlyToOwner => true;
}

[Serializable, NetSerializable]
public sealed class AlertComponentState : ComponentState
{
    public Dictionary<AlertKey, AlertState> Alerts { get; }
    public AlertComponentState(Dictionary<AlertKey, AlertState> alerts)
    {
        Alerts = alerts;
    }
}
