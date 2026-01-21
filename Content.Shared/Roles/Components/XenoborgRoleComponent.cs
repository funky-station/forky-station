// SPDX-FileCopyrightText: 2025 Samuka <47865393+Samuka-C@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Roles.Components;

/// <summary>
/// Added to mind role entities to tag that they are a xenoborg.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class XenoborgRoleComponent : Component;
