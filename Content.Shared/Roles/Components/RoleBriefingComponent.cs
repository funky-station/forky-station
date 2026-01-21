// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Roles.Components;

/// <summary>
/// Adds a briefing to the character info menu, does nothing else.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RoleBriefingComponent : BaseMindRoleComponent
{
    [DataField(required: true), AutoNetworkedField]
    public LocId Briefing;
}
