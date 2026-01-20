// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-License-Identifier: MIT

namespace Content.Shared.Alert;

/// <summary>
///     Raised when the AlertSystem needs alert sources to recalculate their alert states and set them.
/// </summary>
public sealed class AlertSyncEvent : EntityEventArgs
{
    public EntityUid Euid { get; }

    public AlertSyncEvent(EntityUid euid)
    {
        Euid = euid;
    }
}
