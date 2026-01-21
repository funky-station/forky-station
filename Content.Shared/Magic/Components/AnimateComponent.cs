// SPDX-FileCopyrightText: 2025 ActiveMammmoth <140334666+ActiveMammmoth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Magic.Components;

// Added to objects when they are made animate
[RegisterComponent, NetworkedComponent]
public sealed partial class AnimateComponent : Component;
