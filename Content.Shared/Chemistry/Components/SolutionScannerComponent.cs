// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 MisterMecky <mrmecky@hotmail.com>
// SPDX-FileCopyrightText: 2025 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Chemistry.Components;

/// <summary>
/// Allows an entity to examine reagents inside of containers, puddles and similiar via the examine verb.
/// Works when added either directly to an entity or to piece of clothing worn by that entity.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SolutionScannerComponent : Component;

