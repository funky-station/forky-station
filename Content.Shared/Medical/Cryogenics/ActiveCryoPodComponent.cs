// SPDX-FileCopyrightText: 2022 Francesco <frafonia@gmail.com>
// SPDX-FileCopyrightText: 2023, 2025 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Medical.Cryogenics;

/// <summary>
/// Tracking component for an enabled cryo pod (which periodically tries to inject chemicals in the occupant, if one exists)
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActiveCryoPodComponent : Component;
