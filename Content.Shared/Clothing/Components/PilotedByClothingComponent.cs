// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Clothing.EntitySystems;
using Robust.Shared.GameStates;

namespace Content.Shared.Clothing.Components;

/// <summary>
/// Disables client-side physics prediction for this entity.
/// Without this, movement with <see cref="PilotedClothingSystem"/> is very rubberbandy.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PilotedByClothingComponent : Component
{
}
