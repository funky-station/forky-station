// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Roles.Components;

/// <summary>
/// Used on Silicon's minds to get the appropriate mind role
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SiliconBrainRoleComponent : BaseMindRoleComponent;
