// SPDX-FileCopyrightText: 2025 Fildrance <fildrance@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE.Components;

/// <summary>
/// When activated, will shuffle the position of all players
/// within a certain radius.
/// </summary>
[RegisterComponent, Access(typeof(XAEShuffleSystem)), NetworkedComponent, AutoGenerateComponentState]
public sealed partial class XAEShuffleComponent : Component
{
    /// <summary>
    /// Radius, within which mobs would be switched.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Radius = 7.5f;
}
