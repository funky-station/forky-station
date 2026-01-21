// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Ghost;

[RegisterComponent, NetworkedComponent]
public sealed partial class GhostOnMoveComponent : Component
{
    [DataField]
    public bool CanReturn = true;

    [DataField]
    public bool MustBeDead;
}
