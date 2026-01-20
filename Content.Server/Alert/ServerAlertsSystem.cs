// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 SlamBamActionman
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-License-Identifier: MIT

using Content.Shared.Alert;
using Robust.Shared.GameStates;

namespace Content.Server.Alert;

internal sealed class ServerAlertsSystem : AlertsSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AlertsComponent, ComponentGetState>(OnGetState);
    }

    private void OnGetState(Entity<AlertsComponent> alerts, ref ComponentGetState args)
    {
        // TODO: Use sourcegen when clone-state bug fixed.
        args.State = new AlertComponentState(new(alerts.Comp.Alerts));
    }
}
