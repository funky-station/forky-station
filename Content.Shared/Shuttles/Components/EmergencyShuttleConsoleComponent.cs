// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Winkarst-cpu <74284083+Winkarst-cpu@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Shuttles.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class EmergencyShuttleConsoleComponent : Component
{
    /// <summary>
    /// ID cards that have been used to authorize an early launch.
    /// Key is the UID of the ID card,
    /// value is the card's name at the time of authorization.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("authorized")]
    public Dictionary<EntityUid, string> AuthorizedEntities = new();

    [ViewVariables(VVAccess.ReadWrite), DataField("authorizationsRequired")]
    public int AuthorizationsRequired = 3;
}
