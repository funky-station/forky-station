// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Shared.PowerCell.Components;
using Robust.Shared.GameStates;

namespace Content.Shared.UserInterface;

/// <summary>
/// Specifies that the attached entity requires <see cref="PowerCellDrawComponent"/> power to open the activatable UI.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActivatableUIRequiresPowerCellComponent : Component;
