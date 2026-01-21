// SPDX-FileCopyrightText: 2025 Jessica M <jessica@jessicamaybe.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.RatKing.Components;

/// <summary>
/// This is used for entities that can rummage through entities
/// with the <see cref="RummageableComponent"/>
/// </summary>
///
[RegisterComponent, NetworkedComponent]
public sealed partial class RummagerComponent : Component;
