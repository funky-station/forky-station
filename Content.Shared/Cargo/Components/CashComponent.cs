// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Cargo.Components;

/// <summary>
/// Can be inserted into a <see cref="CargoOrderConsoleComponent"/> to increase the station's bank account.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class CashComponent : Component
{

}
