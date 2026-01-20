// SPDX-FileCopyrightText: 2024 Ilya246
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Ghost;

/// <summary>
/// Marker component to identify "ghostly" entities.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SpectralComponent : Component { }
