// SPDX-FileCopyrightText: 2025 Fildrance <fildrance@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Xenoarchaeology.Artifact.XAE.Components;

/// <summary>
/// This is used for using the "knock" spell when the artifact is activated
/// </summary>
[RegisterComponent, Access(typeof(XAEKnockSystem)), NetworkedComponent, AutoGenerateComponentState]
public sealed partial class XAEKnockComponent : Component
{
    /// <summary>
    /// The range of the spell
    /// </summary>
    [DataField, AutoNetworkedField]
    public float KnockRange = 4f;
}
