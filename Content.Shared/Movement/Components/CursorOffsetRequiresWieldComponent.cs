// SPDX-FileCopyrightText: 2025 SlamBamActionman <83650252+SlamBamActionman@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Wieldable;
using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components;

/// <summary>
/// Indicates that this item requires wielding for the cursor offset effect to be active.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedWieldableSystem))]
public sealed partial class CursorOffsetRequiresWieldComponent : Component
{

}
