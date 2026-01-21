// SPDX-FileCopyrightText: 2023-2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.PowerCell.Components;
using Robust.Shared.GameStates;

namespace Content.Shared.UserInterface;

/// <summary>
/// Specifies that the attached entity requires <see cref="PowerCellDrawComponent"/> power to open the activatable UI.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActivatableUIRequiresPowerCellComponent : Component;
