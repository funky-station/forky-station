// SPDX-FileCopyrightText: 2023-2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aaron <betterwithketchup@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Shuttles.Components;

/// <summary>
/// Add to grids that you do not want to FTL, but still might want to pilot.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PreventFTLComponent : Component;
